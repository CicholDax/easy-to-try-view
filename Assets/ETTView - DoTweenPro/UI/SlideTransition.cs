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
	public class SlideTransition : Reopnable
	{
		enum WAY
		{
			SLIDE_LEFT,
			SLIDE_RIGHT
		}
		[SerializeField] float _duration;
		[SerializeField] Graphic _targetGraphic;
		[SerializeField] WAY _way;

		RectTransform _canvasRectt;
		RectTransform CanvasRectt
		{
			get
			{
				if (_canvasRectt == null)
					_canvasRectt = _targetGraphic.canvas.GetComponent<RectTransform>();

				return _canvasRectt;
			}
		}

		public override async UniTask Loading(CancellationToken token)
		{
			if (transform != null)
				transform.localPosition = new Vector2((_way == WAY.SLIDE_RIGHT ? -1 : 1) * -CanvasRectt.sizeDelta.x, 0.0f);
			await base.Loading(token);
		}

		public override async UniTask Opening(CancellationToken token)
		{
			await base.Opening(token);

            if (transform != null)
                transform.localPosition = new Vector2((_way == WAY.SLIDE_RIGHT ? -1 : 1) * -CanvasRectt.sizeDelta.x, 0.0f);
			await transform.DOLocalMoveX(0, _duration);
		}

		public override async UniTask Closing(CancellationToken token)
		{
			await base.Closing(token);

            if (transform != null)
                await transform.DOLocalMoveX((_way == WAY.SLIDE_RIGHT ? -1 : 1) * CanvasRectt.sizeDelta.x, _duration);
		}
	}
}