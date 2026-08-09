using System.Linq;
using Cysharp.Threading.Tasks;
using ETTView.UI;
using TMPro;
using UnityEngine;

namespace ETTView.Samples.Showcase
{
    /// <summary>
    /// Showcase のルートビュー。前進遷移・ポップアップ・状態遷移の入口を提供し、
    /// Reopener の Phase / ビュー履歴数 / 現在ステート / ポップアップ有無をライブ表示する。
    /// シーンに最初から置くトップビュー（_isSceneTopView = true）を想定。
    /// Back（ESC）は UIViewManager が拾うため自前実装は不要。
    /// </summary>
    public class ShowcaseView : UIView
    {
        [SerializeField] TextMeshProUGUI _statusText;
        [SerializeField] ShowcaseState _stateA;
        [SerializeField] ShowcaseState _stateB;

        /// <summary>Open Detail ボタン。DetailView を前進遷移で開く（現在ビューは自動 Close）。</summary>
        public void OnOpenDetail()
        {
            DetailView.Create(transform.parent).Forget();
        }

        /// <summary>Show Popup ボタン。メッセージポップアップを開く。</summary>
        public void OnShowPopup()
        {
            MessagePopup.Create(transform, "Confirm", "This is the popup body.\nPress OK to close.").Forget();
        }

        /// <summary>State A ボタン。UIViewState を A に切り替える（状態履歴に積まれる）。</summary>
        public void OnStateA()
        {
            _stateA.OpenNowait();
        }

        /// <summary>State B ボタン。UIViewState を B に切り替える。</summary>
        public void OnStateB()
        {
            _stateB.OpenNowait();
        }

        void Update()
        {
            if (_statusText == null) return;

            var current = Current;
            var state = StateHistory.FirstOrDefault();
            var stateLabel = state is ShowcaseState showcaseState ? showcaseState.Label : state != null ? state.name : "-";

            _statusText.text =
                $"Phase        : {Phase}\n" +
                $"View History : {History.Count()}\n" +
                $"Current View : {(current != null ? current.name : "-")}\n" +
                $"Current State: {stateLabel}\n" +
                $"Popup        : {(ExistPopup() ? "open" : "none")}";
        }
    }
}
