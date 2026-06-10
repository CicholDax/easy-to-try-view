using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ETTView
{
    /// <summary>
    /// 低解像度UIのあたり判定用GraphicRaycaster。
    /// Start時にCanvasのRender CameraをLowResCamに自動設定し、
    /// クリック座標はスクリーン解像度から低解像度仮想座標に変換してレイキャストする。
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class LowResGraphicRaycaster : GraphicRaycaster
    {
        protected override void Start()
        {
            base.Start();
            var manager = LowResManager.Instance;
            if (manager != null)
                GetComponent<Canvas>().worldCamera = manager.LowResCam;
        }

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            var manager = LowResManager.InstanceOrNull;
            if (manager == null)
            {
                base.Raycast(eventData, resultAppendList);
                return;
            }

            var original = eventData.position;
            eventData.position = manager.ScreenToLowResPoint(original);
            base.Raycast(eventData, resultAppendList);
            eventData.position = original;
        }
    }
}
