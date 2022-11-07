using System;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Rendering;
using SharpDX;

namespace TestDraven
{
    internal class Draven
    {
        public Draven()
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }

        private static AIHeroClient Player => ObjectManager.Player;
        private static Menu _dravenMenu;
        private static Spell _q, _w, _e, _r;

        // Menu components
        private static MenuBool _comboE = new MenuBool("_comboE", "Use E on combo");

        private static MenuBool _drawERange = new MenuBool("_drawERange", "Draw E Range");
        private void OnGameLoad()
        {
            if (Player.CharacterName != "Draven") return;

            Game.Print("Draven Loaded"); 
            Console.WriteLine("Draven Loaded VUIScript"); 
            _q = new Spell(SpellSlot.Q);
            _w = new Spell(SpellSlot.W);
            _e = new Spell(SpellSlot.E,  260f);
            _e.SetSkillshot(.25f * 4, 260f, float.MaxValue, false, SpellType.Line);
            _r = new Spell(SpellSlot.R, float.MaxValue);
            _r.SetSkillshot(.5f * 4, 320f, float.MaxValue, false, SpellType.Line);

            // Menu
            _dravenMenu = new Menu("draven", "Draven", true);
            var comboMenu = new Menu("combo", "Combo")
            {
                _comboE
            };
            _dravenMenu.Add(comboMenu);

            var drawMenu = new Menu("draw", "Draw")
            {
                _drawERange
            };
            _dravenMenu.Add(drawMenu);

            _dravenMenu.Attach();

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
            if (_comboE.Enabled && _e.IsReady())
            {
                var eTarget = TargetSelector.GetTarget(_e.Range, DamageType.Physical);
                _ = _e.GetTarget();
                if (eTarget != null)
                {
                    var eInput = _e.GetPrediction(eTarget);
                    if (eInput.Hitchance >= HitChance.VeryHigh)
                    {
                        _e.Cast(eInput.CastPosition);
                    }
                }
            }
        }
    }
}