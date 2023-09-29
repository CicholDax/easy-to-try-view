using Cysharp.Threading.Tasks;
using ETTView;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupReflector : Reopnable
{
	CanvasGroup _canvasGroup;

	public void Awake()
	{
		_canvasGroup = GetComponent<CanvasGroup>();
	}

	public override async UniTask Opening(CancellationToken token)
	{
		_canvasGroup.interactable = true;
		_canvasGroup.blocksRaycasts = true;
	}

	public override async UniTask Closing()
	{
		_canvasGroup.interactable = false;
		_canvasGroup.blocksRaycasts = false;
	}
}
