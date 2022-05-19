using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using DG.Tweening;
using UniDax;

namespace UniDax.UI
{
	public class TweenTransition : Reopnable
	{
		[SerializeField] public DOTweenAnimations _startTween;
		[SerializeField] public DOTweenAnimations _endTween;

		public override async UniTask Opening()
		{
			await base.Opening();

			if (_startTween != null && !_startTween.IsEmpty())
			{
				await _startTween.PlayForward();
			}
			else
			{
				await UniTask.WaitForEndOfFrame();
			}
		}

		public override async UniTask Closing()
		{
			await base.Closing();

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