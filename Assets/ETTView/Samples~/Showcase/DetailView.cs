using Cysharp.Threading.Tasks;
using ETTView.UI;
using UnityEngine;

namespace ETTView.Samples.Showcase
{
    /// <summary>
    /// Root(ShowcaseView) から前進遷移で開かれる詳細ビュー。
    /// Resources プレハブから生成され、Preopning でマネージャに登録されると現在ビューは自動 Close される。
    /// Back（ボタン or ESC）で前のビューへ戻る。
    /// </summary>
    public class DetailView : UIView
    {
        /// <summary>Resources/DetailView プレハブから生成する。</summary>
        public static UniTask<DetailView> Create(Transform parent)
        {
            return CreateFromResources<DetailView>(parent);
        }

        /// <summary>← Back ボタンから呼ぶ。前のビューへ戻る（ESC でも戻れる）。</summary>
        public void OnClickBack()
        {
            BackForget();
        }
    }
}
