using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using ETTView;

namespace ETTView.UI
{
	[RequireComponent(typeof(CanvasGroup))]
	public class FadeTransition : Reopenable
	{
		[SerializeField] float _duration = 1.0f;

		[SerializeField] float _closeAlpha = 0.0f;
		[SerializeField] float _openAlpha = 1.0f;

		private CanvasGroup _canvasGroup;
		
		public void Awake()
		{
			_canvasGroup = GetComponent<CanvasGroup>();
		}

		public override async UniTask Loading(CancellationToken token)
		{
			_canvasGroup.alpha = _closeAlpha;
			_canvasGroup.interactable = false;
			await base.Loading(token);
		}

		public override async UniTask Opening(CancellationToken token)
		{
            await base.Opening(token);
            await _canvasGroup.DOFade(_openAlpha, _duration);
            
            _canvasGroup.interactable = true;
		}

		public override async UniTask Closing(CancellationToken token)
		{
			_canvasGroup.interactable = false;
			
            await base.Closing(token);
            await _canvasGroup.DOFade(_closeAlpha, _duration);
		}
	}
}