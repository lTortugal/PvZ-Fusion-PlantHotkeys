using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(HotkeyMod.Mod), "PlantHotkeys", "1.1", "Tortuga")]

namespace HotkeyMod
{
    public class Mod : MelonMod
    {
        static readonly string[] Ids =
        {
            "Slot1", "Slot2", "Slot3", "Slot4", "Slot5", "Slot6", "Slot7", "Slot8", "Slot9", "Slot10", "Slot11", "Slot12", "Slot13", "Slot14",
            "Shovel", "Glove", "Slow", "Hammer", "Wheel", "PlantHP", "ZombieHP", "GoldBean", "BulletDmg", "FullScreen"
        };
        static readonly string[] ToolTitles =
        {
            "Лопата", "Перчатка", "Замедление", "Молоток", "Колесо", "HP растений", "HP зомби", "Золотой боб", "Урон пуль", "Полный экран"
        };
        static readonly KeyCode[] Defaults =
        {
            KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5,
            KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0,
            KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R,
            KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.Tab, KeyCode.LeftShift, KeyCode.T, KeyCode.Y, KeyCode.F, KeyCode.U, KeyCode.CapsLock
        };

        static readonly Dictionary<string, KeyCode> Keys = new Dictionary<string, KeyCode>();
        static readonly HashSet<KeyCode> ModKeys = new HashSet<KeyCode> { KeyCode.F10, KeyCode.F11, KeyCode.Escape };
        static readonly HashSet<KeyCode> GameKeys = new HashSet<KeyCode> { KeyCode.C, KeyCode.H, KeyCode.G, KeyCode.P, KeyCode.X, KeyCode.V, KeyCode.B, KeyCode.F1 };
        static readonly List<KeyCode> AllKeys = BuildKeys();

        // клавиша, которую прячем от игры (зашитый сброс растения)
        public static readonly KeyCode HiddenKey = KeyCode.R;
        public static bool Allow = false;

        static bool guiOpen = false;
        static string listening = null;
        static string status = "";
        static float statusUntil = 0f;

        bool useClickOnCard = true;
        string lastErr = "";

        public static KeyCode Get(string id) { return Keys[id]; }
        public static string Name(string id) { return Pretty(Keys[id]); }

        static string Pretty(KeyCode k)
        {
            string n = k.ToString();
            if (n.StartsWith("Alpha")) return n.Substring(5);
            if (n.StartsWith("Keypad")) return "Num" + n.Substring(6);
            return n;
        }

        static string Title(int i) { return i < 14 ? "Слот " + (i + 1) : ToolTitles[i - 14]; }

        static List<KeyCode> BuildKeys()
        {
            var l = new List<KeyCode>();
            foreach (var v in Enum.GetValues(typeof(KeyCode)))
            {
                var k = (KeyCode)v;
                if ((int)k > 0 && (int)k < (int)KeyCode.Mouse0) l.Add(k);
            }
            return l;
        }

        static void ResetDefaults()
        {
            for (int i = 0; i < Ids.Length; i++) Keys[Ids[i]] = Defaults[i];
        }

        static void SetStatus(string s) { status = s; statusUntil = Time.unscaledTime + 4f; }

        // ===== сохранение и загрузка =====
        static string CfgPath()
        {
            string dir;
            try { dir = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "UserData")); }
            catch (Exception) { dir = Path.Combine(Directory.GetCurrentDirectory(), "UserData"); }
            Directory.CreateDirectory(dir);
            return Path.Combine(dir, "PlantHotkeys.cfg");
        }

        static void Save()
        {
            try
            {
                var lines = new List<string>();
                lines.Add("# PlantHotkeys: клавиши. Формат: Действие=Клавиша (имена Unity KeyCode: Alpha1, Q, F5, Keypad1...)");
                lines.Add("# Файл можно править руками, пока игра закрыта. Удали файл, чтобы вернуть стандартную раскладку.");
                foreach (var id in Ids) lines.Add(id + "=" + Keys[id]);
                File.WriteAllLines(CfgPath(), lines);
            }
            catch (Exception e) { MelonLogger.Warning("cfg save: " + e.Message); }
        }

        static void Load()
        {
            try
            {
                string p = CfgPath();
                if (!File.Exists(p)) { Save(); return; }
                foreach (var line in File.ReadAllLines(p))
                {
                    string s = line.Trim();
                    if (s.Length == 0 || s.StartsWith("#")) continue;
                    int eq = s.IndexOf('=');
                    if (eq < 0) continue;
                    string id = s.Substring(0, eq).Trim();
                    string val = s.Substring(eq + 1).Trim();
                    if (!Keys.ContainsKey(id)) continue;
                    KeyCode k;
                    if (Enum.TryParse<KeyCode>(val, true, out k) && (int)k > 0) Keys[id] = k;
                }
            }
            catch (Exception e) { MelonLogger.Warning("cfg load: " + e.Message); }
        }

        public override void OnInitializeMelon()
        {
            ResetDefaults();
            Load();
            LoggerInstance.Msg("PlantHotkeys 1.1 loaded");
        }

        // читаем клавишу так, чтобы наш перехват для игры нас не скрывал
        static bool Down(KeyCode k)
        {
            Allow = true;
            try { return Input.GetKeyDown(k); }
            finally { Allow = false; }
        }

        public override void OnUpdate()
        {
            try
            {
                if (Down(KeyCode.F11)) { guiOpen = !guiOpen; listening = null; }
                HandleClick();
                if (listening != null) { Rebind(); return; }

                ApplyKeys();
                LabelFix.Tick();
                CardLabels.Tick();

                
                if (Down(KeyCode.F10))
                {
                    useClickOnCard = !useClickOnCard;
                    LoggerInstance.Msg("click method: " + (useClickOnCard ? "Mouse.ClickOnCard" : "CardUI.OnMouseDown"));
                }

                if (Il2Cpp.InGameUI.Instance == null) return;

                for (int i = 0; i < 14; i++)
                    if (Down(Keys[Ids[i]])) Pick(i);
            }
            catch (Exception e)
            {
                string m = e.GetType().Name + ": " + e.Message;
                if (m != lastErr) { lastErr = m; LoggerInstance.Error(e.ToString()); }
            }
        }

        // ===== переназначение =====
        static void Rebind()
        {
            if (Down(KeyCode.Escape)) { listening = null; return; }
            for (int n = 0; n < AllKeys.Count; n++)
            {
                var k = AllKeys[n];
                bool pressed;
                try { pressed = Down(k); }
                catch (Exception e) { MelonLogger.Warning("key " + k + ": " + e.Message); continue; }
                if (!pressed) continue;
                TryAssign(k);
                return;
            }
        }

        static void TryAssign(KeyCode k)
        {
            if (listening == null) return;
            if (ModKeys.Contains(k)) { SetStatus("Эта клавиша занята самим модом (F10, F11)"); return; }
            MelonLogger.Msg("rebind " + listening + " -> " + k);
            Assign(listening, k);
            listening = null;
        }

        static void Assign(string id, KeyCode k)
        {
            foreach (var other in Ids)
            {
                if (other != id && Keys[other] == k) { Keys[other] = Keys[id]; break; }
            }
            Keys[id] = k;
            Save();
            SetStatus(GameKeys.Contains(k) ? "Внимание: клавишу " + Pretty(k) + " использует сама игра" : "Сохранено");
        }

        // ===== окно настроек =====
        public override void OnGUI()
        {
            if (!guiOpen) return;
            var old = GUI.matrix;
            try { DrawGui(); }
            catch (Exception) { }
            finally { GUI.matrix = old; }
        }

        static void DrawGui()
        {
            float s = Scale();
            GUI.matrix = Matrix4x4.Scale(new Vector3(s, s, 1f));
            var w = WinRect(s);

            for (int i = 0; i < 3; i++) GUI.Box(w, "");
            GUI.Label(new Rect(w.x + 15f, w.y + 8f, w.width - 30f, 26f), "PlantHotkeys 1.1 by Tortuga: настройка клавиш (F11: закрыть)");

            for (int i = 0; i < Ids.Length; i++)
            {
                var b = RowButton(w, i);
                GUI.Label(new Rect(b.x - 125f, b.y, 125f, 26f), Title(i));
                GUI.Button(b, listening == Ids[i] ? "нажми клавишу..." : Pretty(Keys[Ids[i]]));
            }
            GUI.Button(ResetRect(w), "Сбросить всё");
            GUI.Button(CloseRect(w), "Закрыть");

            string st = Time.unscaledTime < statusUntil ? status : "Нажми кнопку, затем клавишу. Esc: отмена. Занятая клавиша меняется местами.";
            GUI.Label(new Rect(w.x + 15f, w.y + w.height - 26f, w.width - 30f, 22f), st);
        }

        static float Scale()
        {
            float s = Screen.height / 1080f;
            return s < 0.75f ? 0.75f : s;
        }

        static Rect WinRect(float s)
        {
            float W = 580f, H = 520f;
            return new Rect((Screen.width / s - W) / 2f, (Screen.height / s - H) / 2f, W, H);
        }

        static Rect RowButton(Rect w, int i)
        {
            int col = i < 14 ? 0 : 1;
            int row = i < 14 ? i : i - 14;
            return new Rect(w.x + 15f + col * 285f + 125f, w.y + 42f + row * 29f, 135f, 26f);
        }

        static Rect ResetRect(Rect w) { return new Rect(w.x + 15f, w.y + w.height - 62f, 150f, 30f); }
        static Rect CloseRect(Rect w) { return new Rect(w.x + w.width - 165f, w.y + w.height - 62f, 150f, 30f); }

        // клики по окну считаем сами, по положению мыши
        static void HandleClick()
        {
            if (!guiOpen || !Input.GetMouseButtonDown(0)) return;
            float s = Scale();
            var m = Input.mousePosition;
            var p = new Vector2(m.x / s, (Screen.height - m.y) / s);
            var w = WinRect(s);

            for (int i = 0; i < Ids.Length; i++)
            {
                if (RowButton(w, i).Contains(p))
                {
                    listening = Ids[i];
                    MelonLogger.Msg("listening " + Ids[i]);
                    return;
                }
            }
            if (ResetRect(w).Contains(p))
            {
                ResetDefaults(); Save(); listening = null; SetStatus("Раскладка сброшена");
                return;
            }
            if (CloseRect(w).Contains(p)) { guiOpen = false; listening = null; }
        }

        // ===== выбор карточки =====
        void Pick(int index)
        {
            var mgr = Il2Cpp.InGameUI.Instance.CardSlotManager;
            Il2Cpp.CardUI card = mgr != null ? mgr.GetCardAtIndex(index) : null;
            if (card == null) return;

            var mouse = Il2Cpp.Mouse.Instance;
            if (mouse != null)
            {
                var held = mouse.theCardOnMouse;
                if (held != null && held.Pointer != card.Pointer)
                {
                    try
                    {
                        held.PutDown();
                        var pv = mouse.preview;
                        var it = mouse.theItemOnMouse;
                        if (pv != null) UnityEngine.Object.Destroy(pv);
                        if (it != null) UnityEngine.Object.Destroy(it);
                        mouse.preview = null;
                        mouse.theItemOnMouse = null;
                    }
                    catch (Exception e) { LoggerInstance.Error("switch cleanup: " + e.Message); }
                }
            }

            if (useClickOnCard) Il2Cpp.Mouse.Instance.ClickOnCard(card);
            else card.OnMouseDown();
        }

        // ===== клавиши самой игры =====
        static void ApplyKeys()
        {
            var K = Keys;
            if (Il2Cpp.KeyCodeManager.Shovel != K["Shovel"]) Il2Cpp.KeyCodeManager.Shovel = K["Shovel"];
            if (Il2Cpp.KeyCodeManager.Glove != K["Glove"]) Il2Cpp.KeyCodeManager.Glove = K["Glove"];
            if (Il2Cpp.KeyCodeManager.SlowTrigger != K["Slow"]) Il2Cpp.KeyCodeManager.SlowTrigger = K["Slow"];
            if (Il2Cpp.KeyCodeManager.Hammer != K["Hammer"]) Il2Cpp.KeyCodeManager.Hammer = K["Hammer"];
            if (Il2Cpp.KeyCodeManager.Wheel != K["Wheel"]) Il2Cpp.KeyCodeManager.Wheel = K["Wheel"];
            if (Il2Cpp.KeyCodeManager.ShowPlantHealth != K["PlantHP"]) Il2Cpp.KeyCodeManager.ShowPlantHealth = K["PlantHP"];
            if (Il2Cpp.KeyCodeManager.ShowZombieHealth != K["ZombieHP"]) Il2Cpp.KeyCodeManager.ShowZombieHealth = K["ZombieHP"];
            if (Il2Cpp.KeyCodeManager.UseGoldBean != K["GoldBean"]) Il2Cpp.KeyCodeManager.UseGoldBean = K["GoldBean"];
            if (Il2Cpp.KeyCodeManager.ShowBulletDamage != K["BulletDmg"]) Il2Cpp.KeyCodeManager.ShowBulletDamage = K["BulletDmg"];
            if (Il2Cpp.KeyCodeManager.FullScreen != K["FullScreen"]) Il2Cpp.KeyCodeManager.FullScreen = K["FullScreen"];
        }
    }

    [HarmonyPatch(typeof(Input), "GetKeyDown", new Type[] { typeof(KeyCode) })]
    static class HideKeyDown
    {
        static bool Prefix(KeyCode key, ref bool __result)
        {
            if (key == Mod.HiddenKey && !Mod.Allow) { __result = false; return false; }
            return true;
        }
    }

    [HarmonyPatch(typeof(Input), "GetKey", new Type[] { typeof(KeyCode) })]
    static class HideKey
    {
        static bool Prefix(KeyCode key, ref bool __result)
        {
            if (key == Mod.HiddenKey && !Mod.Allow) { __result = false; return false; }
            return true;
        }
    }

    [HarmonyPatch(typeof(Input), "GetKeyUp", new Type[] { typeof(KeyCode) })]
    static class HideKeyUp
    {
        static bool Prefix(KeyCode key, ref bool __result)
        {
            if (key == Mod.HiddenKey && !Mod.Allow) { __result = false; return false; }
            return true;
        }
    }
}
