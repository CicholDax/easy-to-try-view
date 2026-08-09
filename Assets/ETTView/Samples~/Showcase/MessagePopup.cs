using Cysharp.Threading.Tasks;
using ETTView.UI;
using TMPro;
using UnityEngine;

namespace ETTView.Samples.Showcase
{
    /// <summary>
    /// タイトル / 本文を TMP で表示し、OK ボタンで閉じる汎用メッセージポップアップ。
    /// 旧 CommonPopup（UnityEngine.UI.Text）の後継。
    /// Resources プレハブから生成する CreateFromResources ワークフローの実例。
    /// </summary>
    public class MessagePopup : UIViewPopup
    {
        [SerializeField] TextMeshProUGUI _title;
        [SerializeField] TextMeshProUGUI _body;

        /// <summary>Resources/MessagePopup プレハブから生成してタイトル・本文を差し込む。</summary>
        public static async UniTask<MessagePopup> Create(Transform parent, string title, string body)
        {
            var ins = await CreateFromResources<MessagePopup>(parent);
            ins._title.text = title;
            ins._body.text = body;
            return ins;
        }

        /// <summary>OK ボタンから呼ぶ。最後に開いたポップアップとして閉じる。</summary>
        public void OnClickClose()
        {
            BackForget();
        }
    }
}
