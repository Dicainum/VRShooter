using UnityEngine;
using UnityEngine.UI;

namespace VRTraining.UI
{
    // Панели собираются кодом, чтобы срать префабами
    public static class UiFactory
    {
        public static readonly Color PanelColor = new Color(0.07f, 0.09f, 0.12f, 0.92f);
        public static readonly Color AccentColor = new Color(1f, 0.72f, 0.15f);
        public static readonly Color TextColor = new Color(0.93f, 0.95f, 0.97f);
        public static readonly Color ButtonColor = new Color(0.16f, 0.36f, 0.58f);

        public static Font DefaultFont => Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        public static RectTransform CreateRect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = (RectTransform)go.transform;
            rect.SetParent(parent, false);

            return rect;
        }

        public static Image CreatePanel(string name, Transform parent, Color color, bool raycastTarget = false)
        {
            var rect = CreateRect(name, parent);
            var image = rect.gameObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = raycastTarget;

            return image;
        }

        public static Text CreateText(string name, Transform parent, string content, int fontSize,
            TextAnchor alignment = TextAnchor.UpperLeft, FontStyle style = FontStyle.Normal)
        {
            var rect = CreateRect(name, parent);
            var text = rect.gameObject.AddComponent<Text>();
            text.font = DefaultFont;
            text.text = content;
            text.fontSize = fontSize;
            text.fontStyle = style;
            text.color = TextColor;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.supportRichText = false;
            text.raycastTarget = false;

            return text;
        }

        public static Button CreateButton(string name, Transform parent, string label, int fontSize = 28)
        {
            var image = CreatePanel(name, parent, ButtonColor, raycastTarget: true);
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;

            var colors = button.colors;
            colors.highlightedColor = new Color(0.25f, 0.5f, 0.78f);
            colors.pressedColor = new Color(0.1f, 0.25f, 0.42f);
            colors.selectedColor = colors.highlightedColor;
            colors.fadeDuration = 0.08f;
            button.colors = colors;

            var text = CreateText("Label", image.transform, label, fontSize, TextAnchor.MiddleCenter, FontStyle.Bold);
            Stretch((RectTransform)text.transform);

            return button;
        }

        public static void Stretch(RectTransform rect, float padding = 0f)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(padding, padding);
            rect.offsetMax = new Vector2(-padding, -padding);
        }
    }
}