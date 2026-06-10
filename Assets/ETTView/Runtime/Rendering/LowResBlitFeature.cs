using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
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
            _pass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var manager = LowResManager.InstanceOrNull;
            if (manager == null || manager.DisplayCamera == null) return;
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
            RTHandle      _sourceHandle;
            RenderTexture _cachedRT;

            class PassData
            {
                public RTHandle Source;
                public int ScreenW, ScreenH, RTW, RTH;
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                var manager = LowResManager.InstanceOrNull;
                if (manager?.LowResRT == null) return;

                if (manager.LowResRT != _cachedRT)
                {
                    _sourceHandle?.Release();
                    _sourceHandle = RTHandles.Alloc(manager.LowResRT);
                    _cachedRT     = manager.LowResRT;
                }

                var resourceData = frameData.Get<UniversalResourceData>();
                var cameraData   = frameData.Get<UniversalCameraData>();
                var desc         = cameraData.cameraTargetDescriptor;

                using (var builder = renderGraph.AddRasterRenderPass<PassData>("LowResBlit", out var passData))
                {
                    passData.Source  = _sourceHandle;
                    passData.ScreenW = desc.width;
                    passData.ScreenH = desc.height;
                    passData.RTW     = manager.LowResRT.width;
                    passData.RTH     = manager.LowResRT.height;

                    // ReadWrite: DisplayCamera のクリア済み黒背景を保持しつつ書き込む
                    builder.SetRenderAttachment(resourceData.activeColorTexture, 0, AccessFlags.ReadWrite);
                    // 外部 RT はレンダーグラフ外でカメラが書き込み済みのため ImportTexture 不要
                    builder.AllowPassCulling(false);

                    builder.SetRenderFunc(static (PassData data, RasterGraphContext ctx) =>
                    {
                        ctx.cmd.SetViewport(CalcLetterbox(data.RTW, data.RTH, data.ScreenW, data.ScreenH));
                        Blitter.BlitTexture(ctx.cmd, data.Source, new Vector4(1, 1, 0, 0), 0, false);
                    });
                }
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
