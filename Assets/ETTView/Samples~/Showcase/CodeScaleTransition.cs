using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ETTView.Samples.Showcase
{
    /// <summary>
    /// DOTween / Animator に依存せず、コードだけで書いたスケール＋フェードのトランジション。
    /// Opening で 0.85→1.0 に拡大しながらフェードイン、Closing で 1.0→0.9 に縮小しながらフェードアウトする。
    /// ライブラリ内蔵の <see cref="ETTView.UI.AnimatorTransition"/> に対する「コードだけ版」の実例。
    /// UIView の _forwardTransitions / _rewindTransitions に登録して前進・巻き戻しを出し分ける。
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class CodeScaleTransition : Reopenable
    {
        [SerializeField] float _duration = 0.25f;
        [SerializeField] float _openFromScale = 0.85f;
        [SerializeField] float _closeToScale = 0.9f;

        CanvasGroup _canvasGroup;
        RectTransform _rect;

        protected override void OnAwake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _rect = transform as RectTransform;
        }

        public override async UniTask Opening(CancellationToken token)
        {
            await Tween(_openFromScale, 1f, 0f, 1f, token);
        }

        public override async UniTask Closing(CancellationToken token)
        {
            await Tween(1f, _closeToScale, 1f, 0f, token);
        }

        async UniTask Tween(float fromScale, float toScale, float fromAlpha, float toAlpha, CancellationToken token)
        {
            var t = 0f;
            while (t < _duration)
            {
                token.ThrowIfCancellationRequested();

                t += Time.unscaledDeltaTime;
                var p = Mathf.Clamp01(t / _duration);

                _rect.localScale = Vector3.one * Mathf.LerpUnclamped(fromScale, toScale, EaseOutBack(p));
                _canvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, p);

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            _rect.localScale = Vector3.one * toScale;
            _canvasGroup.alpha = toAlpha;
        }

        static float EaseOutBack(float x)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            var m = x - 1f;
            return 1f + c3 * m * m * m + c1 * m * m;
        }
    }
}
