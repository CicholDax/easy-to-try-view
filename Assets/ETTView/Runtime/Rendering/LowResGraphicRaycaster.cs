using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ETTView
{
    /// <summary>
    /// 低解像度UIのあたり判定用GraphicRaycaster。
    /// Canvasのレンダリングは LowResCam（256×192）で行いつつ、
    /// クリック座標はスクリーン解像度から低解像度仮想座標に変換してレイキャストする。
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class LowResGraphicRaycaster : GraphicRaycaster
    {
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
