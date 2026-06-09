using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ETTView
{
    /// <summary>
    /// LowResManager が生成した低解像度RTをレターボックス付きで画面に出力する RendererFeature。
    /// URP Renderer Asset の Renderer Features に追加し、LowResManager をシーンに置くだけで動作する。
    /// </summary>
    public class LowResBlitFeature : ScriptableRendererFeature
    {
        LowResBlitPass _pass;

        public override void Create()
        {
            _pass = new LowResBlitPass();
            // PostProcessing 後・Screen Space Overlay UI 描画前のタイミング
            _pass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            // LowResManager の DisplayCamera でのみ動作する
            var manager = LowResManager.Instance;
            if (manager == null) return;
            if (renderingData.cameraData.camera != manager.DisplayCamera) return;

            renderer.EnqueuePass(_pass);
        }

        protected override void Dispose(bool disposing)
        {
            _pass?.Dispose();
        }

        // ─────────────────────────────────────────────────────────────────

        class LowResBlitPass : ScriptableRenderPass, IDisposable
        {
            RTHandle _sourceHandle;
            RenderTexture _cachedRT;

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                var manager = LowResManager.Instance;
                if (manager == null || manager.LowResRT == null) return;

                // RenderTexture → RTHandle のラッパーを更新
                if (manager.LowResRT != _cachedRT)
                {
                    _sourceHandle?.Release();
                    _sourceHandle = RTHandles.Alloc(manager.LowResRT);
                    _cachedRT     = manager.LowResRT;
                }

                var cmd    = CommandBufferPool.Get("LowResBlit");
                var color  = renderingData.cameraData.renderer.cameraColorTargetHandle;
                var desc   = renderingData.cameraData.cameraTargetDescriptor;
                int sw = desc.width;
                int sh = desc.height;

                // レターボックス矩形を計算（ピクセル座標）
                Rect vp = CalcLetterbox(manager.LowResRT.width, manager.LowResRT.height, sw, sh);

                // 低解像度RT → DisplayCamera の出力（画面）にアップスケールBlit
                // bilinear=false でポイントフィルタリング（ドット絵維持）
                Blitter.BlitCameraTexture(cmd, _sourceHandle, color, vp, 0, false);

                // 次のパスのためにビューポートをフル解像度に戻す
                cmd.SetViewport(new Rect(0, 0, sw, sh));

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            static Rect CalcLetterbox(int rtW, int rtH, int sw, int sh)
            {
                float ta = (float)rtW / rtH;
                float sa = (float)sw / sh;
                float w, h, x, y;
                if (sa > ta) { h = sh; w = h * ta; x = (sw - w) / 2f; y = 0;              }
                else         { w = sw; h = w / ta; x = 0;              y = (sh - h) / 2f; }
                return new Rect(x, y, w, h);
            }

            public void Dispose()
            {
                _sourceHandle?.Release();
                _sourceHandle = null;
                _cachedRT     = null;
            }
        }
    }
}
