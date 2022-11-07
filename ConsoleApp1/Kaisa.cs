using System;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Rendering;
using SharpDX;

namespace TestKaisa
{
    internal class Kaisa
    {
        public Kaisa()
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }

        private static AIHeroClient Player => ObjectManager.Player;
        private static Menu _KaisaMenu;
        private static Spell _q, _w, _e, _r;

        // Menu components
        private static MenuBool _comboW = new MenuBool("_comboW", "Use W on combo");
        private static MenuBool _comboQ = new MenuBool("_comboQ", "Use Q on combo");
        private static MenuBool _drawWRange = new MenuBool("_drawWRange", "Draw W Range");
        private static MenuBool _drawQRange = new MenuBool("_drawQRange", "Draw Q Range");
        private void OnGameLoad()
        {
            if (Player.CharacterName != "Kai'sa") return;

            Game.Print("Kaisa Loaded");
            Console.WriteLine("Kaisa Loaded VUIScript");
            _q = new Spell(SpellSlot.Q);
            _w = new Spell(SpellSlot.W);
            _w.SetSkillshot(.4f * 4, 100f, float.MaxValue, false, SpellType.Line);
            _e = new Spell(SpellSlot.E);
            _r = new Spell(SpellSlot.R, 1500 + 750 * (GameObjects.Player.Spellbook.GetSpell(SpellSlot.R).Level - 1));

            // Menu
            _KaisaMenu = new Menu("kaisa", "Kaisa", true);
            var comboMenu = new Menu("combo", "Combo")
            {
                _comboW,
                _comboQ
            };
            _KaisaMenu.Add(comboMenu);

            var drawMenu = new Menu("draw", "Draw")
            {
                _drawWRange,
                _drawQRange
            };
            _KaisaMenu.Add(drawMenu);

            _KaisaMenu.Attach();

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
                case OrbwalkerMode.Combo:
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
                    if (qInput.Hitchance >= HitChance.Medium)
                    {
                        _q.Cast(qInput.CastPosition);
                    }
                }
            }
            if (_comboW.Enabled && _w.IsReady())
            {
                var wTarget = TargetSelector.GetTarget(_w.Range, DamageType.Magical);
                _ = _w.GetTarget();
                if (wTarget != null)
                {
                    var wInput = _w.GetPrediction(wTarget);
                    if (wInput.Hitchance >= HitChance.VeryHigh)
                    {
                        _w.Cast(wInput.CastPosition);
                    }
                }
            }
        }
    }
}