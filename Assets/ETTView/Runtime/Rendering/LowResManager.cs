using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace ETTView
{
    /// <summary>
    /// 固定解像度のRenderTextureにシーンを描画し、レターボックス付きで画面に表示するマネージャ。
    /// </summary>
    public class LowResManager : SingletonMonoBehaviour<LowResManager>, ISingletonMono
    {
        public bool IsDontDestroy { get; } = true;

        [SerializeField] int _renderWidth = 256;
        [SerializeField] int _renderHeight = 192;
        [SerializeField] FilterMode _filterMode = FilterMode.Point;

        RenderTexture _rt;
        RawImage _display;
        int _prevScreenWidth;
        int _prevScreenHeight;

        protected override void OnAwake()
        {
            _rt = new RenderTexture(_renderWidth, _renderHeight, 24) { filterMode = _filterMode };
            BuildCanvas();
            RenderPipelineManager.beginCameraRendering += OnBeginCamera;
        }

        /// <summary>
        /// メインカメラの描画先をRenderTextureに差し替える。
        /// </summary>
        void OnBeginCamera(ScriptableRenderContext ctx, Camera cam)
        {
            if (cam.CompareTag("MainCamera"))
                cam.targetTexture = _rt;
        }

        /// <summary>
        /// レターボックス表示用のCanvasとRawImageをコードで生成する。
        /// </summary>
        void BuildCanvas()
        {
            var root = new GameObject("LowResDisplay");
            root.transform.SetParent(transform);

            var canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;
            root.AddComponent<CanvasScaler>();

            var bg = new GameObject("BG").AddComponent<Image>();
            bg.color = Color.black;
            bg.transform.SetParent(root.transform, false);
            var bgRect = bg.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = bgRect.offsetMax = Vector2.zero;

            var raw = new GameObject("View").AddComponent<RawImage>();
            raw.texture = _rt;
            raw.transform.SetParent(root.transform, false);
            _display = raw;
        }

        /// <summary>
        /// ウィンドウサイズが変化したときだけレターボックスのサイズを更新する。
        /// </summary>
        void Update()
        {
            if (Screen.width == _prevScreenWidth && Screen.height == _prevScreenHeight) return;
            _prevScreenWidth = Screen.width;
            _prevScreenHeight = Screen.height;

            float targetAspect = (float)_renderWidth / _renderHeight;
            float screenAspect = (float)Screen.width / Screen.height;

            float w, h;
            if (screenAspect > targetAspect)
            {
                h = Screen.height;
                w = h * targetAspect;
            }
            else
            {
                w = Screen.width;
                h = w / targetAspect;
            }

            _display.rectTransform.sizeDelta = new Vector2(w, h);
        }

        void OnDestroy()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCamera;
            if (_rt != null) _rt.Release();
        }
    }
}
