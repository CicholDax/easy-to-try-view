using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ETTView
{
    /// <summary>
    /// 低解像度レンダリングを管理するシングルトン。
    /// MainCamera を表示専用（targetTexture=null）に変更し、
    /// 動的生成した LowResCam でシーンを低解像度RTに描画する。
    /// 最終的な画面出力は LowResBlitFeature が担当する。
    /// </summary>
    public class LowResManager : SingletonMonoBehaviour<LowResManager>, ISingletonMono
    {
        public bool IsDontDestroy { get; } = true;

        [SerializeField] int _renderWidth  = 256;
        [SerializeField] int _renderHeight = 192;

        RenderTexture _rt;
        Camera        _displayCam;  // MainCamera（targetTexture=null、画面出力担当）
        Camera        _lowResCam;   // 動的生成、targetTexture=_rt でシーンを描画

        /// <summary>低解像度シーンが描画されるRenderTexture。</summary>
        public RenderTexture LowResRT     => _rt;

        /// <summary>画面への最終出力を行うカメラ（MainCamera）。</summary>
        public Camera        DisplayCamera => _displayCam;

        /// <summary>スクリーン座標を低解像度仮想座標に変換する。</summary>
        public Vector2 ScreenToLowResPoint(Vector2 screenPos)
        {
            float ta = (float)_renderWidth / _renderHeight;
            float sa = (float)Screen.width / Screen.height;
            float w, h, x, y;
            if (sa > ta) { h = Screen.height; w = h * ta; x = (Screen.width - w) / 2f; y = 0; }
            else         { w = Screen.width;  h = w / ta; x = 0; y = (Screen.height - h) / 2f; }
            return new Vector2(
                (screenPos.x - x) / w * _renderWidth,
                (screenPos.y - y) / h * _renderHeight);
        }

        protected override void OnAwake()
        {
            _displayCam = Camera.main;
            if (_displayCam == null)
            {
                Debug.LogError("[LowResManager] MainCamera が見つかりません。シーンに MainCamera タグのカメラを配置してください。");
                return;
            }

            _rt = new RenderTexture(_renderWidth, _renderHeight, 24) { filterMode = FilterMode.Point };

            // MainCamera の元設定を引き継いで LowResCam を先に作る
            CreateLowResCam();

            // MainCamera は Blit 出力専用に変更（シーン・UIはすべて LowResCam が _rt に描画する）
            _displayCam.cullingMask     = 0;
            _displayCam.clearFlags      = CameraClearFlags.SolidColor;
            _displayCam.backgroundColor = Color.black;
            _displayCam.depth           = 100;
            _displayCam.targetTexture   = null;

            RenderPipelineManager.beginCameraRendering += OnBeginCamera;
        }

        void OnDestroy()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCamera;
            if (_rt != null) _rt.Release();
            if (_lowResCam != null) Destroy(_lowResCam.gameObject);
        }

        void OnBeginCamera(ScriptableRenderContext ctx, Camera cam)
        {
            // DisplayCamera（MainCamera）と Scene ビュー以外の Game カメラを _rt に向ける
            if (cam != _displayCam && cam.cameraType == CameraType.Game)
                cam.targetTexture = _rt;
        }

        void CreateLowResCam()
        {
            var go = new GameObject("LowResCam");
            go.transform.SetParent(_displayCam.transform);
            go.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            _lowResCam                 = go.AddComponent<Camera>();
            _lowResCam.cullingMask     = _displayCam.cullingMask; // UI含む全レイヤーを低解像度で描画
            _lowResCam.clearFlags      = _displayCam.clearFlags;
            _lowResCam.backgroundColor = _displayCam.backgroundColor;
            _lowResCam.fieldOfView     = _displayCam.fieldOfView;
            _lowResCam.nearClipPlane   = _displayCam.nearClipPlane;
            _lowResCam.farClipPlane    = _displayCam.farClipPlane;
            _lowResCam.depth           = _displayCam.depth - 1;
            _lowResCam.targetTexture   = _rt;

            go.AddComponent<UniversalAdditionalCameraData>();
        }
    }
}
