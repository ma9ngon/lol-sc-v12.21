using System;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Rendering;
using SharpDX;

namespace TestJhin
{
    internal class Jhin
    {
        public Jhin()
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }

        private static AIHeroClient Player => ObjectManager.Player;
        private static Menu _JhinMenu;
        private static Spell _q, _w, _e, _r;

        // Menu components
        private static MenuBool _comboQ = new MenuBool("_comboW", "Use W on combo");
        private static MenuBool _comboW = new MenuBool("_comboW", "Use W on combo");
        public static Spell RShot { get; private set; }
        private static MenuBool _drawWRange = new MenuBool("_drawWRange", "Draw W Range");
        private static MenuBool _drawQRange = new MenuBool("_drawQRange", "Draw Q Range");
        private void OnGameLoad()
        {
            if (Player.CharacterName != "Jhin") return;

            Game.Print("Jhin Loaded");
            Console.WriteLine("Jhin Loaded VUIScript");
            _q = new Spell(SpellSlot.Q, 1150f) {AddHitBox = true};
            _q.SetSkillshot(.25f*4,30f,float.MaxValue,true,SpellType.Line);
            _w = new Spell(SpellSlot.W, 5000f);
            _e = new Spell(SpellSlot.E, 1000f);
            _r = new Spell(SpellSlot.R, 1200f);
            RShot = new Spell(SpellSlot.R, 3500f) { AddHitBox = true };
            RShot.SetSkillshot(.25f * 3.8f, 80f, 5000f, false, SpellType.Line);
            // Menu
            _JhinMenu = new Menu("jhin", "Jhin", true);
            var comboMenu = new Menu("combo", "Combo")
            {
                _comboW,
                _comboQ
            };
            _JhinMenu.Add(comboMenu);

            var drawMenu = new Menu("draw", "Draw")
            {
                _drawWRange,
                _drawQRange
            };
            _JhinMenu.Add(drawMenu);

            _JhinMenu.Attach();

            // Events
            GameEvent.OnGameTick += OnGameTick;
            Drawing.OnDraw += OnDraw;
        }
        private void OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (_drawWRange.Enabled)
            {
                CircleRender.Draw(Player.Position, _w.Range, Color.Green, 1, true);
                CircleRender.Draw(Player.Position, _q.Range, Color.Green, 1, true);
                Drawing.DrawCircleIndicator(Player.Position, _w.Range, Color.Green.ToSystemColor());
                Drawing.DrawCircleIndicator(Player.Position, _q.Range, Color.Green.ToSystemColor());
            }
        }

        private void OnGameTick(EventArgs args)
        {
            if (Player.IsDead) return;
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Harass:
                    comboLogic();
                    break;
            }
        }
        private void comboLogic()
        {
            if (_comboQ.Enabled && _q.IsReady())
            {
                var qTarget = TargetSelector.GetTarget(_w.Range, DamageType.Physical);
                _ = _q.GetTarget();
                if (qTarget != null)
                {
                    var qInput = _q.GetPrediction(qTarget);
                    if (qInput.Hitchance >= HitChance.Medium)
                    {
                        _q.Cast(qInput.CastPosition);
                    }
                }
            }
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
        }
    }
}