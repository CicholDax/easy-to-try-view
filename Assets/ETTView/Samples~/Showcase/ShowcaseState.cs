using UnityEngine;

namespace ETTView.Samples.Showcase
{
    /// <summary>
    /// Showcase 内の State A / B を表す薄いラッパ。
    /// UIViewState の登録・状態履歴（Preopning で View.RegistState）をそのまま使い、
    /// 見分け用のラベルだけ持たせている。State Back は UIViewManager が担当する。
    /// </summary>
    public class ShowcaseState : UIViewState
    {
        [SerializeField] string _label;

        /// <summary>状態表示に使う見分け用ラベル。</summary>
        public string Label => _label;
    }
}
