using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ETTView.Samples.Showcase
{
    /// <summary>
    /// コードで TMP テキストや Button を組み立てるための生成ヘルパ。
    /// エディタのシーン生成スクリプトからも実行時からも使える。
    /// プレハブ内の細かい配線を最小化するために UI を動的生成する。
    /// </summary>
    public static class ShowcaseUiBuilder
    {
        /// <summary>親の下に TMP テキストを生成する。</summary>
        public static TextMeshProUGUI Text(
            Transform parent,
            string content,
            float fontSize = 28f,
            TextAlignmentOptions align = TextAlignmentOptions.TopLeft)
        {
            var go = new GameObject("Text", typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = content;
            tmp.fontSize = fontSize;
            tmp.alignment = align;
            tmp.color = Color.white;

            return tmp;
        }

        /// <summary>親の下にラベル付き Button を生成する。onClick は任意（エディタ配線でも可）。</summary>
        public static Button Button(Transform parent, string label, UnityAction onClick = null)
        {
            var go = new GameObject(label + "Button", typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var image = go.AddComponent<Image>();
            image.color = new Color(0.20f, 0.35f, 0.60f, 1f);

            var button = go.AddComponent<Button>();
            if (onClick != null)
            {
                button.onClick.AddListener(onClick);
            }

            var text = Text(go.transform, label, 24f, TextAlignmentOptions.Center);
            Stretch((RectTransform)text.transform);

            return button;
        }

        /// <summary>RectTransform を親いっぱいに広げる。</summary>
        public static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
