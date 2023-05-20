using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETTView.UI;
using ETTView;
using Cysharp.Threading.Tasks;

public class UIViewState : ReopnablePrefab
{
	[SerializeField] bool _awaitCloseState = false;

	public bool AwaitCloseState => _awaitCloseState;

	public override async UniTask Preopning()
	{
		if (_awaitCloseState)
		{
			await UIViewManager.Instance.Current.RegistState(this);
		}
		else
		{
			UIViewManager.Instance.Current.RegistState(this).Forget();
		}
	}
}
