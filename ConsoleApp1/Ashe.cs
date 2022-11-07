using System;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Rendering;
using SharpDX;

namespace TestAshe
{
    internal class Ashe
    {
        public Ashe()
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }

        private static AIHeroClient Player => ObjectManager.Player;
        private static Menu _AsheMenu;
        private static Spell _q, _w, _e, _r;

        // Menu components
        private static MenuBool _comboW = new MenuBool("_comboW", "Use W on combo");

        private static MenuBool _drawWRange = new MenuBool("_drawWRange", "Draw W Range");
        private void OnGameLoad()
        {
            if (Player.CharacterName != "Ashe") return;

            Game.Print("Ashe Loaded");
            Console.WriteLine("Ashe Loaded VUIScript");
            _q = new Spell(SpellSlot.Q);
            _w = new Spell(SpellSlot.W, 1200f);
            _w.SetSkillshot(.25f, 80f, 2000f, true, SpellType.Cone);
            _r = new Spell(SpellSlot.R, 4000);
            _r.SetSkillshot(1f, 125f, 1600f, false, SpellType.Line);

            // Menu
            _AsheMenu = new Menu("ashe", "Ashe", true);
            var comboMenu = new Menu("combo", "Combo")
            {
                _comboW
            };
            _AsheMenu.Add(comboMenu);

            var drawMenu = new Menu("draw", "Draw")
            {
                _drawWRange
            };
            _AsheMenu.Add(drawMenu);

            _AsheMenu.Attach();

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
                Drawing.DrawCircleIndicator(Player.Position, _w.Range, Color.Green.ToSystemColor());
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