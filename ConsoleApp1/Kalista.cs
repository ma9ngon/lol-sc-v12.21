using System;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Rendering;
using SharpDX;

namespace TestKalista
{
    internal class Kalista
    {
        public Kalista()
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }

        private static AIHeroClient Player => ObjectManager.Player;
        private static Menu _KalistaMenu;
        private static Spell _q, _w, _e, _r;

        // Menu components
        private static MenuBool _comboE = new MenuBool("_comboE", "Use E on combo");
        private static MenuBool _comboQ = new MenuBool("_comboE", "Use Q on combo");

        private static MenuBool _drawWRange = new MenuBool("_drawWRange", "Draw E Range");
        private void OnGameLoad()
        {
            if (Player.CharacterName != "Kalista") return;

            Game.Print("Kalista Loaded");
            Console.WriteLine("Kalista Loaded VUIScript");
            _q = new Spell(SpellSlot.Q, 1150f) { AddHitBox = true };
            _q.SetSkillshot(.25f * 4, 30f, float.MaxValue, true, SpellType.Line);
            _w = new Spell(SpellSlot.W, 5000f);
            _e = new Spell(SpellSlot.E, 1000f);
            _r = new Spell(SpellSlot.R, 1200f);

            // Menu
            _KalistaMenu = new Menu("kalista", "Kalista", true);
            var comboMenu = new Menu("combo", "Combo")
            {
                _comboE,
                _comboQ
            };
            _KalistaMenu.Add(comboMenu);

            var drawMenu = new Menu("draw", "Draw")
            {
                _drawWRange
            };
            _KalistaMenu.Add(drawMenu);

            _KalistaMenu.Attach();

            // Events
            GameEvent.OnGameTick += OnGameTick;
            Drawing.OnDraw += OnDraw;
        }
        private void OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (_drawWRange.Enabled)
            {
                CircleRender.Draw(Player.Position, _q.Range, Color.Green, 1, true);
                CircleRender.Draw(Player.Position, _e.Range, Color.Green, 1, true);
                Drawing.DrawCircleIndicator(Player.Position, _q.Range, Color.Green.ToSystemColor());
                Drawing.DrawCircleIndicator(Player.Position, _e.Range, Color.Green.ToSystemColor());
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
                var qTarget = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
                _ = _q.GetTarget();
                if (qTarget != null)
                {
                    var qInput = _q.GetPrediction(qTarget);
                    if (qInput.Hitchance >= HitChance.High)
                    {
                        _q.Cast(qInput.CastPosition);
                    }
                }
            }
            if (_comboE.Enabled && _e.IsReady())
            {
                var eTarget = TargetSelector.GetTarget(_e.Range, DamageType.Physical);
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