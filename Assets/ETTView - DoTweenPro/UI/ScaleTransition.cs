using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using DG.Tweening;
using ETTView;

namespace ETTView.UI
{
	public class ScaleTransition : Reopnable
	{
		[SerializeField] float _duration = 1.0f;

		[SerializeField] float _closeScale = 0.2f;
		[SerializeField] float _openScale = 1.0f;

		public override async UniTask Loading()
		{
			transform.localScale = Vector3.one * _closeScale;
			await base.Loading();
		}

		public override async UniTask Opening(CancellationToken token)
		{
            await base.Opening(token);
            Tween tween = transform.DOScale(_openScale, _duration);

			using (token.Register(() => tween.Kill()))
			{
				await tween;
			}
        }

		public override async UniTask Closing()
		{
			await base.Closing();
			await transform.DOScale(_closeScale, _duration);
		}
	}
}