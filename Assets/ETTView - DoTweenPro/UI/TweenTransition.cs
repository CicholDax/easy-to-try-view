using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using DG.Tweening;
using ETTView;

namespace ETTView.UI
{
	public class TweenTransition : Reopenable
	{
		[SerializeField] public DOTweenAnimations _startTween;
		[SerializeField] public DOTweenAnimations _endTween;

		public override async UniTask Opening(CancellationToken token)
		{
			await base.Opening(token);

			if (_startTween != null && !_startTween.IsEmpty())
			{
				await _startTween.PlayForward();
			}
			else
			{
				await UniTask.WaitForEndOfFrame();
			}
		}

		public override async UniTask Closing(CancellationToken token)
		{
			await base.Closing(token);

			if (_endTween != null && !_endTween.IsEmpty())
			{
				await _endTween.PlayForward();
			}
			else if (_startTween != null && !_startTween.IsEmpty())   //EndTweenが定義されてなかったらスタートを逆再生
			{
				await _startTween.SmoothRewind();
			}
			else
			{
				await UniTask.WaitForEndOfFrame();
			}
		}
	}
}