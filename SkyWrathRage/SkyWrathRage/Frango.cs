﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ensage;
using Ensage.Common.Extensions;
using Ensage.Common;
using SharpDX.Direct3D9;
using SharpDX;
using Ensage.Common.Menu;

namespace SkyWrathRage
{
    class Frango
    {
        private static readonly Menu Menu = new Menu("Skywrath rage", "Skywrath rage", true);
        private static readonly Menu _magic_items = new Menu("Magic Damage Items", "Magic Damage Items");
        private static readonly Menu _amplify_items = new Menu("Magic Amplify Items", "Magic Amplify Items");
        private static readonly Menu _disable_items = new Menu("Disable/slow Items", "Disable/slow Items");
        private static readonly Menu _remove_linkens_items = new Menu("Pop Linkens Items", "Pop Linkens Items");
        private static readonly Menu _skills = new Menu("Skills", "Skills");
        private static Ability bolt, slow, silence, mysticflare;
        private static Item dust, sentry, soulring, force, cyclone, orchid, sheep, veil, shivas, dagon, atos, ethereal;
        private static Hero me, target;
        private static readonly Dictionary<string, bool> magic_damage = new Dictionary<string, bool>
            {
                {"item_shivas_guard",true},
                {"item_dagon",true}
            };
        private static readonly Dictionary<string, bool> magic_damage_amplify = new Dictionary<string, bool>
            {
                {"item_orchid",true},
                {"item_veil_of_discord",true},
                {"item_ethereal_blade",true}
            };
        private static readonly Dictionary<string, bool> disable_slow = new Dictionary<string, bool>
            {
                {"item_rod_of_atos",true},
                {"item_sheepstick",true},
                {"item_orchid",true},
                {"item_ethereal_blade",true}
            };
        private static readonly Dictionary<string, bool> remove_linkens = new Dictionary<string, bool>
            {
                {"item_rod_of_atos",true},
                {"item_sheepstick",true},
                {"skywrath_mage_ancient_seal",true},
                {"skywrath_mage_arcane_bolt",true},
                {"item_force_staff",true},
                {"item_cyclone",true },
                {"item_orchid",true},
                {"item_dagon",true},
                {"item_ethereal_blade",true}
            };
        private static readonly Dictionary<string, bool> skills_menu = new Dictionary<string, bool>
            {
                {"skywrath_mage_arcane_bolt",true},
                {"skywrath_mage_concussive_shot",true},
                {"skywrath_mage_ancient_seal",true},
                {"skywrath_mage_mystic_flare",true}
            };
        private static int[] bolt_damage = new int[4] { 60, 80, 100, 120 };
        static void Main(string[] args)
        {
            Menu.AddItem(new MenuItem("Ultimate Key", "Ultimate Key").SetValue(new KeyBind('F', KeyBindType.Press)));
            Menu.AddItem(new MenuItem("Combo Key", "Combo Key").SetValue(new KeyBind('D', KeyBindType.Press)));
            Menu.AddItem(new MenuItem("Chase Key", "Chase Key").SetValue(new KeyBind('T', KeyBindType.Press)));
            Menu.AddItem(new MenuItem("Soulring", "Soulring").SetValue(true).SetTooltip("Use soulring before use the combo if your HP is greater than 150."));
            Menu.AddSubMenu(_magic_items);
            Menu.AddSubMenu(_amplify_items);
            Menu.AddSubMenu(_disable_items);
            Menu.AddSubMenu(_remove_linkens_items);
            Menu.AddSubMenu(_skills);
            _magic_items.AddItem(new MenuItem("Magic Damage Items", "Magic Damage Items").SetValue(new AbilityToggler(magic_damage)));
            _amplify_items.AddItem(new MenuItem("Magic Amplify Items", "Magic Amplify Items").SetValue(new AbilityToggler(magic_damage_amplify)));
            _disable_items.AddItem(new MenuItem("Disable/slow Items", "Disable/slow Items").SetValue(new AbilityToggler(disable_slow)));
            _remove_linkens_items.AddItem(new MenuItem("Pop Linkens Items", "Pop Linkens Items").SetValue(new AbilityToggler(remove_linkens)));
            _skills.AddItem(new MenuItem("Skills", "Skills").SetValue(new AbilityToggler(skills_menu)));
            Menu.AddToMainMenu();
            // start
            PrintSuccess("> SkyWrath rage Script Injected!");
            Game.OnUpdate += Raging;
            Drawing.OnDraw += Information;
            Game.OnWndProc += Activation;
        }
        public static void Raging(EventArgs args)
        {
            if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame)
                return;
            me = ObjectMgr.LocalHero;
            if (me == null || me.ClassID != ClassID.CDOTA_Unit_Hero_Skywrath_Mage)
                return;
            if (Game.IsKeyDown(Menu.Item("Chase Key").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
            {
                FindItems();
                target = me.ClosestToMouseTarget();
                if (target != null && target.IsValid && target.IsVisible && !target.IsIllusion && target.IsAlive && !me.IsChanneling() && !target.IsInvul())
                {
                    if (Utils.SleepCheck("FASTCOMBO"))
                    {
                        if (bolt.Level > 0 && bolt.CanBeCasted() && !target.IsMagicImmune() && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(bolt.Name))
                        {
                            bolt.UseAbility(target,false);
                            Utils.Sleep(150, "FASTCOMBO");
                        }
                        else
                            me.Attack(target, false);
                    }
                }
                else
                {
                    if (!me.IsChanneling())
                        me.Move(Game.MousePosition, false);
                }
            }
            if (Game.IsKeyDown(Menu.Item("Combo Key").GetValue<KeyBind>().Key) && !Game.IsChatOpen)
            {
                FindItems();
                target = me.ClosestToMouseTarget(1200);
                if (target != null && target.IsValid && target.IsVisible && !target.IsIllusion && target.IsAlive && !me.IsChanneling() && !target.IsInvul())
                {
                    if (target.IsLinkensProtected())
                    {
                        if (Utils.SleepCheck("DistanceDelay"))
                        {
                            if (cyclone != null && cyclone.CanBeCasted() && Menu.Item("Pop Linkens Items").GetValue<AbilityToggler>().IsEnabled(cyclone.Name))
                                cyclone.UseAbility(target, false);
                            else if (force != null && force.CanBeCasted() && Menu.Item("Pop Linkens Items").GetValue<AbilityToggler>().IsEnabled(force.Name))
                                force.UseAbility(target, false);
                            else if (orchid != null && orchid.CanBeCasted() && Menu.Item("Pop Linkens Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name))
                                orchid.UseAbility(target, false);
                            else if (atos != null && atos.CanBeCasted() && Menu.Item("Pop Linkens Items").GetValue<AbilityToggler>().IsEnabled(atos.Name))
                                atos.UseAbility(target, false);
                            else if (silence.Level >= 1 && silence.CanBeCasted() && Menu.Item("Pop Linkens Items").GetValue<AbilityToggler>().IsEnabled(silence.Name))
                                silence.UseAbility(target, false);
                            else if (bolt.Level >= 1 && bolt.CanBeCasted() && Menu.Item("Pop Linkens Items").GetValue<AbilityToggler>().IsEnabled(bolt.Name))
                            {
                                bolt.UseAbility(target, false);
                                Utils.Sleep(me.NetworkPosition.Distance2D(target.NetworkPosition) / 500 * 1000, "DistanceDelay");
                            }
                            else if (dagon != null && dagon.CanBeCasted() && Menu.Item("Pop Linkens Items").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
                                dagon.UseAbility(target, false);
                            else if (ethereal != null && ethereal.CanBeCasted() && Menu.Item("Pop Linkens Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
                            {
                                ethereal.UseAbility(target, false);
                                Utils.Sleep(me.NetworkPosition.Distance2D(target.NetworkPosition) / 1200 * 1000, "DistanceDelay");
                            }
                            else if (sheep != null && sheep.CanBeCasted() && Menu.Item("Pop Linkens Items").GetValue<AbilityToggler>().IsEnabled(sheep.Name))
                                sheep.UseAbility(target, false);
                        }

                    }
                    else
                    {
                        if (Utils.SleepCheck("FASTCOMBO"))
                        {
                            bool EzkillCheck = EZkill(), MagicImune = target.IsMagicImmune();
                            uint elsecount = 0;
                            if (soulring != null && soulring.CanBeCasted() && Menu.Item("Soulring").GetValue<bool>())
                                soulring.UseAbility(false);
                            else
                                elsecount += 1;
                            if (sheep != null && sheep.CanBeCasted() && !target.UnitState.HasFlag(UnitState.Hexed) && !target.UnitState.HasFlag(UnitState.Stunned) && !MagicImune && Menu.Item("Disable/slow Items").GetValue<AbilityToggler>().IsEnabled(sheep.Name))
                                sheep.UseAbility(target, false);
                            else
                                elsecount += 1;
                            if (silence.Level > 0 && silence.CanBeCasted() && !MagicImune && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(silence.Name))
                                silence.UseAbility(target, false);
                            else
                                elsecount += 1;
                            if (atos != null && atos.CanBeCasted() && !MagicImune && target.MovementSpeed >= 200 && Menu.Item("Disable/slow Items").GetValue<AbilityToggler>().IsEnabled(atos.Name))
                                atos.UseAbility(target, false);
                            else
                                elsecount += 1;
                            if (slow.Level > 0 && slow.CanBeCasted() && target.NetworkPosition.Distance2D(me) <= 1600 && !MagicImune && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(slow.Name))
                            {
                                slow.UseAbility(false);
                                if (Utils.SleepCheck("SlowDelay") && me.Distance2D(target) <= slow.CastRange)
                                    Utils.Sleep(((me.NetworkPosition.Distance2D(target.NetworkPosition) / 500) * 1000), "SlowDelay");
                            }
                            else
                                elsecount += 1;
                            if (orchid != null && orchid.CanBeCasted() && !MagicImune && Menu.Item("Magic Amplify Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name))
                                orchid.UseAbility(target, false);
                            else
                                elsecount += 1;
                            if (veil != null && veil.CanBeCasted() && !MagicImune && Menu.Item("Magic Amplify Items").GetValue<AbilityToggler>().IsEnabled(veil.Name))
                                veil.UseAbility(target.NetworkPosition, false);
                            else
                                elsecount += 1;
                            if (shivas != null && shivas.CanBeCasted() && !EzkillCheck && !MagicImune && Menu.Item("Magic Damage Items").GetValue<AbilityToggler>().IsEnabled(shivas.Name))
                                shivas.UseAbility(false);
                            else
                                elsecount += 1;
                            if (ethereal != null && ethereal.CanBeCasted() && (!veil.CanBeCasted() || veil == null | !Menu.Item("Magic Amplify Items").GetValue<AbilityToggler>().IsEnabled(veil.Name)) && !MagicImune && Menu.Item("Magic Amplify Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
                            {
                                ethereal.UseAbility(target, false);
                                if (Utils.SleepCheck("EtherealDelay") && me.Distance2D(target) <= ethereal.CastRange)
                                    Utils.Sleep(((me.NetworkPosition.Distance2D(target.NetworkPosition) / 1200) * 1000), "EtherealDelay");
                            }
                            else
                                elsecount += 1;
                            if (dagon != null && dagon.CanBeCasted() && (!veil.CanBeCasted() || veil == null | !Menu.Item("Magic Amplify Items").GetValue<AbilityToggler>().IsEnabled(veil.Name)) && !silence.CanBeCasted() && !ethereal.CanBeCasted() && Utils.SleepCheck("EtherealDelay") && !MagicImune && Menu.Item("Magic Damage Items").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
                                dagon.UseAbility(target, false);
                            else
                                elsecount += 1;
                            if (bolt.Level > 0 && bolt.CanBeCasted() && !EzkillCheck && !MagicImune && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(bolt.Name))
                                bolt.UseAbility(target, false);
                            else
                                elsecount += 1;
                            if (mysticflare.Level > 0 && mysticflare.CanBeCasted() && (Utils.SleepCheck("EtherealDelay")
                                && Utils.SleepCheck("SlowDelay") || target.UnitState.HasFlag(UnitState.Frozen) || target.UnitState.HasFlag(UnitState.Stunned) || target.MovementSpeed <= 280)
                                && !EzkillCheck && Utils.SleepCheck("MysticDamaging")
                                && (target.DamageTaken((int)(bolt_damage[bolt.Level - 1] + (me.TotalIntelligence * 1.6)), DamageType.Magical, me, false, 0, 0, 0) * 2 <= target.Health
                                || me.Health <= (int)(me.MaximumHealth * 0.35))
                                && (!atos.CanBeCasted() || atos == null | Menu.Item("Disable/slow Items").GetValue<AbilityToggler>().IsEnabled(atos.Name)) && (!slow.CanBeCasted() || !Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(slow.Name)) && (!ethereal.CanBeCasted() || ethereal == null | !Menu.Item("Magic Amplify Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name)) && !MagicImune && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled("skywrath_mage_mystic_flare"))
                            {
                                if (!target.CanMove() || target.NetworkActivity == NetworkActivity.Idle)
                                    mysticflare.UseAbility(target.NetworkPosition, false);
                                else
                                    mysticflare.UseAbility(Prediction.PredictedXYZ(target, (220 / target.MovementSpeed) * 1000), false);
                                int[] mysticflaredamage = new int[3] { 600, 1000, 1400 };
                                if (target.Health <= target.DamageTaken(mysticflaredamage[mysticflare.Level - 1], DamageType.Magical, me, false, 0, 0, 0))
                                    Utils.Sleep(2500, "MysticDamaging");
                            }
                            else
                                elsecount += 1;
                            if (elsecount == 12)
                                me.Attack(target,false);
                            Utils.Sleep(150, "FASTCOMBO");
                        }
                    }
                }
                else
                {
                    if (!me.IsChanneling())
                        me.Move(Game.MousePosition, false);
                }
            }
        }
        static bool IsLinkensProtected(Hero x)
        {
            if (x.Modifiers.Any(m => m.Name == "modifier_item_sphere_target") || x.FindItem("item_sphere") != null && x.FindItem("item_sphere").Cooldown <= 0)
                return true;
            else
                return false;
        }
        static void FindItems()
        {
            if (Utils.SleepCheck("FindItems"))
            {
                dust = me.FindItem("item_dust");
                sentry = me.FindItem("item_sentry");
                soulring = me.FindItem("item_soul_ring");
                force = me.FindItem("item_force_staff");
                cyclone = me.FindItem("item_cyclone");
                orchid = me.FindItem("item_orchid");
                sheep = me.FindItem("item_sheepstick");
                veil = me.FindItem("item_veil_of_discord");
                shivas = me.FindItem("item_shivas_guard");
                dagon = me.Inventory.Items.FirstOrDefault(item => item.Name.Contains("item_dagon"));
                atos = me.FindItem("item_rod_of_atos");
                ethereal = me.FindItem("item_ethereal_blade");
                bolt = me.Spellbook.SpellQ;
                slow = me.Spellbook.SpellW;
                silence = me.Spellbook.SpellE;
                mysticflare = me.Spellbook.SpellR;
                Utils.Sleep(300, "FindItems");
            }
        }
        static bool EZkill()
        {
            if (target != null && target.IsAlive && target.IsValid)
            {
                int alldamage = 0, percent = 0, orchidpercent = 0;
                int[] dagondamage = new int[5] { 400, 500, 600, 700, 800 };
                int etherealdamage = (int)(me.TotalIntelligence * 2) + 75;
                if (orchid != null && orchid.CanBeCasted() && Menu.Item("Magic Amplify Items").GetValue<AbilityToggler>().IsEnabled(orchid.Name))
                    alldamage += 30;
                if (ethereal != null && ethereal.CanBeCasted() && Menu.Item("Magic Amplify Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
                    percent += 40;
                if (veil != null && veil.CanBeCasted() && Menu.Item("Magic Amplify Items").GetValue<AbilityToggler>().IsEnabled(veil.Name))
                    percent += 25;
                if (silence.Level > 0 && silence.CanBeCasted() && Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled(silence.Name))
                    percent += (int)((silence.Level - 1) * 5) + 30;
                if (dagon != null && dagon.CanBeCasted() && Menu.Item("Magic Damage Items").GetValue<AbilityToggler>().IsEnabled("item_dagon"))
                    alldamage += (int)target.DamageTaken(dagondamage[dagon.Level - 1], DamageType.Magical, me, false, 0, orchidpercent, percent);
                if (ethereal != null && ethereal.CanBeCasted() && Menu.Item("Magic Amplify Items").GetValue<AbilityToggler>().IsEnabled(ethereal.Name))
                    alldamage += (int)target.DamageTaken(etherealdamage, DamageType.Magical, me, false, 0, 0, 25);
                if (target.Health < alldamage)
                    return true;
                else
                    return false;
            }
            else
                return false;

        }
        static void Activation(EventArgs args)
        {
            if (Game.IsKeyDown(Menu.Item("Ultimate Key").GetValue<KeyBind>().Key) && !Game.IsChatOpen && Utils.SleepCheck("Ultimate Key"))
            {
                skills_menu["skywrath_mage_mystic_flare"] = !Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled("skywrath_mage_mystic_flare");
                Utils.Sleep(400, "Ultimate Key");
            }
        }
        static void Information(EventArgs args)
        {
            if (!Game.IsInGame || Game.IsWatchingGame)
                return;
            me = ObjectMgr.LocalHero;
            if (me == null)
                return;
            if (me.ClassID != ClassID.CDOTA_Unit_Hero_Skywrath_Mage)
                return;
            target = me.ClosestToMouseTarget(2000);
            if (target != null && target.IsValid && !target.IsIllusion && target.IsAlive && target.IsVisible)
            {
                FindItems();
                Drawing.DrawText(EZkill() ? "EZKILL" : "Simple Target", HeroPositionOnScreen(target), new Vector2(18, 200), EZkill() ? Color.ForestGreen : Color.MintCream, FontFlags.AntiAlias | FontFlags.Additive | FontFlags.DropShadow);
            }
            if (!Utils.SleepCheck("Ultimate Key"))
                Drawing.DrawText(Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled("skywrath_mage_mystic_flare") == true ? "ON" : "OFF", new Vector2(HUDInfo.ScreenSizeX() / 2, HUDInfo.ScreenSizeY() / 2), new Vector2(30, 200), Menu.Item("Skills").GetValue<AbilityToggler>().IsEnabled("skywrath_mage_mystic_flare") == true ? Color.LimeGreen : Color.Red, FontFlags.AntiAlias | FontFlags.Additive | FontFlags.DropShadow);
        }
        static Vector2 HeroPositionOnScreen(Hero x)
        {
            Vector2 PicPosition;
            PicPosition = new Vector2(HUDInfo.GetHPbarPosition(x).X - 1, HUDInfo.GetHPbarPosition(x).Y - 40);
            return PicPosition;
        }
        private static void PrintSuccess(string text, params object[] arguments)
        {
            PrintEncolored(text, ConsoleColor.Green, arguments);
        }
        private static void PrintEncolored(string text, ConsoleColor color, params object[] arguments)
        {
            var clr = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text, arguments);
            Console.ForegroundColor = clr;
        }
    }
}
