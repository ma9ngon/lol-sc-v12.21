using System;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Rendering;
using SharpDX;

namespace TestJinx
{
    internal class Jinx
    {
        public Jinx()
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }

        private static AIHeroClient Player => ObjectManager.Player;
        private static Menu _JinxMenu;
        private static Spell _q, _w, _e, _r;

        // Menu components
        private static MenuBool _comboE = new MenuBool("_comboE", "Use E on combo");
        private static MenuBool _comboW = new MenuBool("_comboW", "Use W on combo");

        private static MenuBool _drawERange = new MenuBool("_drawERange", "Draw Q Range");
        private void OnGameLoad()
        {
            if (Player.CharacterName != "Jinx") return;

            Game.Print("Jinx Loaded");
            Console.WriteLine("Jinx Loaded VUIScript");
            _q = new Spell(SpellSlot.Q);
            _w = new Spell(SpellSlot.W);
            _w.SetSkillshot(.6f * 4, 120f, 3300f, false, SpellType.Line);
            _e = new Spell(SpellSlot.E, 260f);
            _e.SetSkillshot(.7f * 4, 300f, float.MaxValue, false, SpellType.Circle);
            _r = new Spell(SpellSlot.R, float.MaxValue);
            _r.SetSkillshot(.6f * 4, 100f, float.MaxValue, false, SpellType.Line);

            // Menu
            _JinxMenu = new Menu("jinx", "Jinx", true);
            var comboMenu = new Menu("combo", "Combo")
            {
                _comboE,
                _comboW
            };
            _JinxMenu.Add(comboMenu);

            var drawMenu = new Menu("draw", "Draw")
            {
                _drawERange
            };
            _JinxMenu.Add(drawMenu);

            _JinxMenu.Attach();

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
                case OrbwalkerMode.Combo:
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
                    if (eInput.Hitchance >= HitChance.High)
                    {
                        _e.Cast(eInput.CastPosition);
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