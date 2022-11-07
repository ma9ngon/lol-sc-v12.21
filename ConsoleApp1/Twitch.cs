using System;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Rendering;
using SharpDX;

namespace TestTwitch
{
    internal class Twitch
    {
        public Twitch()
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }

        private static AIHeroClient Player => ObjectManager.Player;
        private static Menu _TwitchMenu;
        private static Spell _q, _w, _e, _r;

        // Menu components
        private static MenuBool _comboE = new MenuBool("_comboE", "Use E on combo");
        private static MenuBool _comboW = new MenuBool("_comboW", "Use W on combo");

        private static MenuBool _drawERange = new MenuBool("_drawERange", "Draw E Range");
        private static MenuBool _drawWRange = new MenuBool("_drawWRange", "Draw W Range");
        private void OnGameLoad()
        {
            if (Player.CharacterName != "Twitch") return;

            Game.Print("Twitch Loaded");
            Console.WriteLine("Twitch Loaded VUIScript");
            _q = new Spell(SpellSlot.Q);
            _w = new Spell(SpellSlot.W, 950f);
            _e = new Spell(SpellSlot.E, 1200f);
            _r = new Spell(SpellSlot.R, 850f);

            // Menu
            _TwitchMenu = new Menu("twitch", "Twitch", true);
            var comboMenu = new Menu("combo", "Combo")
            {
                _comboE,
                _comboW
            };
            _TwitchMenu.Add(comboMenu);

            var drawMenu = new Menu("draw", "Draw")
            {
                _drawERange,
                _drawWRange
            };
            _TwitchMenu.Add(drawMenu);

            _TwitchMenu.Attach();

            // Events
            GameEvent.OnGameTick += OnGameTick;
            Drawing.OnDraw += OnDraw;
        }

        private void OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (_drawERange.Enabled)
            {
                CircleRender.Draw(Player.Position, _e.Range, Color.Green, 1, true);
                CircleRender.Draw(Player.Position, _w.Range, Color.Green, 1, true);
                Drawing.DrawCircleIndicator(Player.Position, _e.Range, Color.Green.ToSystemColor());
                Drawing.DrawCircleIndicator(Player.Position, _w.Range, Color.Green.ToSystemColor());
            }
        }

        private void OnGameTick(EventArgs args)
        {
            if (Player.IsDead) return;
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    comboLogic();
                    break;
            }
        }
        private void comboLogic()
        {
            if (_comboW.Enabled && _w.IsReady())
            {
                var wTarget = TargetSelector.GetTarget(_w.Range, DamageType.Physical);
                _ = _w.GetTarget();
                if (wTarget != null)
                {
                    var wInput = _w.GetPrediction(wTarget);
                    if (wInput.Hitchance >= HitChance.High)
                    {
                        _w.Cast(wInput.CastPosition);
                    }
                }
            }
            if (_comboE.Enabled && _e.IsReady())
            {
                var eTarget = TargetSelector.GetTarget(_e.Range, DamageType.Mixed);
                _ = _e.GetTarget();
                if (eTarget != null)
                {
                    var eInput = _e.GetPrediction(eTarget);
                    if (eInput.Hitchance >= HitChance.High)
                    {
                        _e.Cast(eInput.CastPosition);
                    }
                }
            }
        }
    }
}