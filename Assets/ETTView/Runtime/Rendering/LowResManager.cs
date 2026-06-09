using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace ETTView
{
    /// <summary>
    /// 固定解像度のRenderTextureにシーンを描画し、レターボックス付きで画面に表示するマネージャ。
    /// UIを作る場合は DisplayCamera を Screen Space - Camera キャンバスのカメラに設定すること。
    /// </summary>
    public class LowResManager : SingletonMonoBehaviour<LowResManager>, ISingletonMono
    {
        public bool IsDontDestroy { get; } = true;

        [SerializeField] int _renderWidth = 256;
        [SerializeField] int _renderHeight = 192;
        [SerializeField] FilterMode _filterMode = FilterMode.Point;

        RenderTexture _rt;
        RawImage _display;
        Camera _displayCam;
        int _prevScreenWidth;
        int _prevScreenHeight;

        /// <summary>
        /// UIキャンバス（Screen Space - Camera）に設定するカメラ。
        /// このカメラは targetTexture=null で動作するためレイキャスト座標が正しく機能する。
        /// </summary>
        public Camera DisplayCamera => _displayCam;

        protected override void OnAwake()
        {
            _rt = new RenderTexture(_renderWidth, _renderHeight, 24) { filterMode = _filterMode };
            BuildDisplayCamera();
            BuildCanvas();
            RenderPipelineManager.beginCameraRendering += OnBeginCamera;
        }

        /// <summary>
        /// MainCamera の描画先を RT に差し替える。DisplayCamera は対象外。
        /// </summary>
        void OnBeginCamera(ScriptableRenderContext ctx, Camera cam)
        {
            if (cam.CompareTag("MainCamera"))
                cam.targetTexture = _rt;
        }

        /// <summary>
        /// 画面出力専用カメラを生成する。
        /// 3Dオブジェクトは描画せず黒でクリアするだけなので、レターボックスの背景色になる。
        /// </summary>
        void BuildDisplayCamera()
        {
            var go = new GameObject("LowResDisplayCam");
            go.transform.SetParent(transform);
            _displayCam = go.AddComponent<Camera>();
            _displayCam.clearFlags       = CameraClearFlags.SolidColor;
            _displayCam.backgroundColor  = Color.black;
            _displayCam.cullingMask      = 0;
            _displayCam.depth            = 10;
            _displayCam.targetTexture    = null;
        }

        /// <summary>
        /// RTを表示するキャンバスとRawImageを生成する。
        /// </summary>
        void BuildCanvas()
        {
            var root = new GameObject("LowResDisplay");
            root.transform.SetParent(transform);

            var canvas = root.AddComponent<Canvas>();
            canvas.renderMode  = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = _displayCam;
            canvas.sortingOrder = 0;
            root.AddComponent<CanvasScaler>();

            var raw = new GameObject("View").AddComponent<RawImage>();
            raw.texture       = _rt;
            raw.raycastTarget = false;
            raw.transform.SetParent(root.transform, false);
            _display = raw;
        }

        /// <summary>
        /// ウィンドウサイズが変化したときだけレターボックスのサイズを更新する。
        /// </summary>
        void Update()
        {
            if (Screen.width == _prevScreenWidth && Screen.height == _prevScreenHeight) return;
            _prevScreenWidth  = Screen.width;
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
