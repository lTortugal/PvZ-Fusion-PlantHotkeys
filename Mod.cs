using System;
using HarmonyLib;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(HotkeyMod.Mod), "PlantHotkeys", "1.0", "Tortuga")]

namespace HotkeyMod
{
    public class Mod : MelonMod
    {
        // ===== КЛАВИШИ СЛОТОВ: индекс в массиве = номер слота (0-13) =====
        static readonly KeyCode[] SlotKeys =
        {
            KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5,
            KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0,
            KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R
        };

        // ===== ПЕРЕНАЗНАЧЕННЫЕ КЛАВИШИ ИГРЫ =====
        internal static readonly KeyCode ShovelKey      = KeyCode.A;
        internal static readonly KeyCode GloveKey       = KeyCode.S;
        internal static readonly KeyCode SlowKey        = KeyCode.D;
        internal static readonly KeyCode PlantHpKey     = KeyCode.T;
        internal static readonly KeyCode ZombieHpKey    = KeyCode.Y;
        internal static readonly KeyCode HammerKey      = KeyCode.M;
        internal static readonly KeyCode WheelKey       = KeyCode.K;
        internal static readonly KeyCode GoldBeanKey    = KeyCode.F;
        internal static readonly KeyCode BulletDmgKey   = KeyCode.U;
        internal static readonly KeyCode FullScreenKey  = KeyCode.O;

        // Клавиша, которую прячем от игры (зашитый сброс растения)
        public static readonly KeyCode HiddenKey = KeyCode.R;
        public static bool Allow = false;   // true только пока мод сам читает клавишу
        // =========================================

        bool useClickOnCard = true;
        string lastErr = "";

        public override void OnInitializeMelon() => LoggerInstance.Msg("PlantHotkeys 1.0 loaded");

        bool Down(KeyCode k)
        {
            Allow = true;
            try { return Input.GetKeyDown(k); }
            finally { Allow = false; }
        }

        public override void OnUpdate()
        {
            try
            {
                ApplyKeys();
                LabelFix.Tick();

                if (Down(KeyCode.F10))
                {
                    useClickOnCard = !useClickOnCard;
                    LoggerInstance.Msg("click method: " + (useClickOnCard ? "Mouse.ClickOnCard" : "CardUI.OnMouseDown"));
                }

                if (Il2Cpp.InGameUI.Instance == null) return;

                for (int i = 0; i < SlotKeys.Length; i++)
                    if (Down(SlotKeys[i])) Pick(i);
            }
            catch (Exception e)
            {
                string m = e.GetType().Name + ": " + e.Message;
                if (m != lastErr) { lastErr = m; LoggerInstance.Error(e.ToString()); }
            }
        }

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
                        string info = "switch " + held.thePlantType + " -> " + card.thePlantType;
                        held.PutDown();

                        var pv = mouse.preview;
                        var it = mouse.theItemOnMouse;
                        info += " | preview=" + (pv != null ? pv.name : "null") + " item=" + (it != null ? it.name : "null");
                        if (pv != null) UnityEngine.Object.Destroy(pv);
                        if (it != null) UnityEngine.Object.Destroy(it);
                        mouse.preview = null;
                        mouse.theItemOnMouse = null;
                        LoggerInstance.Msg(info);
                    }
                    catch (Exception e) { LoggerInstance.Error("switch cleanup: " + e.Message); }
                }
            }

            if (useClickOnCard) Il2Cpp.Mouse.Instance.ClickOnCard(card);
            else card.OnMouseDown();
        }

        void ApplyKeys()
        {
            if (Il2Cpp.KeyCodeManager.Shovel != ShovelKey) Il2Cpp.KeyCodeManager.Shovel = ShovelKey;
            if (Il2Cpp.KeyCodeManager.Glove != GloveKey) Il2Cpp.KeyCodeManager.Glove = GloveKey;
            if (Il2Cpp.KeyCodeManager.SlowTrigger != SlowKey) Il2Cpp.KeyCodeManager.SlowTrigger = SlowKey;
            if (Il2Cpp.KeyCodeManager.Hammer != HammerKey) Il2Cpp.KeyCodeManager.Hammer = HammerKey;
            if (Il2Cpp.KeyCodeManager.Wheel != WheelKey) Il2Cpp.KeyCodeManager.Wheel = WheelKey;
            if (Il2Cpp.KeyCodeManager.ShowPlantHealth != PlantHpKey) Il2Cpp.KeyCodeManager.ShowPlantHealth = PlantHpKey;
            if (Il2Cpp.KeyCodeManager.ShowZombieHealth != ZombieHpKey) Il2Cpp.KeyCodeManager.ShowZombieHealth = ZombieHpKey;
            if (Il2Cpp.KeyCodeManager.UseGoldBean != GoldBeanKey) Il2Cpp.KeyCodeManager.UseGoldBean = GoldBeanKey;
            if (Il2Cpp.KeyCodeManager.ShowBulletDamage != BulletDmgKey) Il2Cpp.KeyCodeManager.ShowBulletDamage = BulletDmgKey;
            if (Il2Cpp.KeyCodeManager.FullScreen != FullScreenKey) Il2Cpp.KeyCodeManager.FullScreen = FullScreenKey;
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

