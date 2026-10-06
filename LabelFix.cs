using System;
using UnityEngine;

namespace HotkeyMod
{
    static class LabelFix
    {
        static float next = 0f;

        public static void Tick()
        {
            if (Time.unscaledTime < next) return;
            next = Time.unscaledTime + 0.3f;
            try
            {
                var all = UnityEngine.Object.FindObjectsOfType<Il2CppTMPro.TextMeshProUGUI>(true);
                foreach (var t in all)
                {
                    string old = t.text;
                    if (string.IsNullOrEmpty(old)) continue;
                    string pn = t.transform.parent != null ? t.transform.parent.name : "";
                    string nw = null;

                    if (t.name == "text")
                    {
                        if (pn == "ShovelBank") nw = "Клавиша: " + Mod.ShovelKey;
                        else if (pn == "GloveBank") nw = "Клавиша: " + Mod.GloveKey;
                        else if (pn == "HammerBank") nw = "Клавиша: " + Mod.HammerKey;
                        else if (pn == "WheelBank") nw = "Клавиша: " + Mod.WheelKey;
                        else if (pn == "SlowTrigger") nw = "Замедление (" + Mod.SlowKey + ")";
                        else if (pn == "ShowCards" && old.Contains("\u80cc\u5305")) nw = "Рюкзак (B)";
                    }
                    else if (t.name == "BeanCount")
                    {
                        nw = old.Replace("(E)", "(" + Mod.GoldBeanKey + ")");
                    }

                    if (nw != null && nw != old) t.text = nw;
                }
            }
            catch (Exception) { }
        }
    }
}
