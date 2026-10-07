using System;
using UnityEngine;

namespace HotkeyMod
{
    static class CardLabels
    {
        const string LabelName = "PH_KeyLabel";

        // подстройка внешнего вида (доли высоты карточки)
        const float OffsetK = 0.02f;   // насколько опустить подпись под карточку
        const float HeightK = 0.30f;   // высота области подписи

        static float next = 0f;

        public static void Tick()
        {
            if (Time.unscaledTime < next) return;
            next = Time.unscaledTime + 0.3f;
            try
            {
                var ui = Il2Cpp.InGameUI.Instance;
                if (ui == null) return;
                var mgr = ui.CardSlotManager;
                if (mgr == null) return;

                Il2CppTMPro.TextMeshProUGUI tpl = null;
                for (int i = 0; i < 14; i++)
                {
                    try
                    {
                        var card = mgr.GetCardAtIndex(i);
                        if (card == null) continue;
                        Apply(card, Mod.Name("Slot" + (i + 1)), ref tpl);
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception) { }
        }

        static void Apply(Il2Cpp.CardUI card, string key, ref Il2CppTMPro.TextMeshProUGUI tpl)
        {
            var parent = card.transform;
            var existing = parent.Find(LabelName);
            if (existing == null)
            {
                if (tpl == null) tpl = FindTemplate();
                if (tpl == null) return;
                var go = UnityEngine.Object.Instantiate(tpl.gameObject, parent);
                go.name = LabelName;
                go.SetActive(true);
                Layout(go, card);
                existing = go.transform;
            }
            var tmp = existing.GetComponent<Il2CppTMPro.TextMeshProUGUI>();
            if (tmp != null && tmp.text != key) tmp.text = key;
        }

        // шаблон: подпись «Клавиша: ...» у лопаты, перчатки, молотка или колеса
        static Il2CppTMPro.TextMeshProUGUI FindTemplate()
        {
            Il2CppTMPro.TextMeshProUGUI fallback = null;
            foreach (var t in UnityEngine.Object.FindObjectsOfType<Il2CppTMPro.TextMeshProUGUI>(true))
            {
                if (t.name != "text" || t.transform.parent == null) continue;
                string pn = t.transform.parent.name;
                if (pn != "ShovelBank" && pn != "GloveBank" && pn != "HammerBank" && pn != "WheelBank") continue;
                if (t.gameObject.activeInHierarchy) return t;
                if (fallback == null) fallback = t;
            }
            return fallback;
        }

        // подпись висит под нижним краем карточки
        static void Layout(GameObject go, Il2Cpp.CardUI card)
        {
            var rt = go.GetComponent<RectTransform>();
            var cr = card.GetComponent<RectTransform>();
            if (rt == null || cr == null) return;
            float cw = cr.rect.width, ch = cr.rect.height;
            rt.anchorMin = new Vector2(0.5f, 0f);
            rt.anchorMax = new Vector2(0.5f, 0f);
            rt.pivot = new Vector2(0.5f, 1f);
            rt.sizeDelta = new Vector2(cw, ch * HeightK);
            rt.anchoredPosition = new Vector2(0f, -ch * OffsetK);
            rt.localScale = Vector3.one;
            var tmp = go.GetComponent<Il2CppTMPro.TextMeshProUGUI>();
            if (tmp != null) tmp.alignment = Il2CppTMPro.TextAlignmentOptions.Center;
        }
    }
}
