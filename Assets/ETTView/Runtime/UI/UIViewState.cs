using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETTView.UI;
using ETTView;
using Cysharp.Threading.Tasks;
using System.Threading;

public class UIViewState : ReopnablePrefab
{
	[SerializeField] bool _awaitCloseState = false;

	public UIView View => UIViewManager.Instance.Current;

	public bool AwaitCloseState => _awaitCloseState;

	public async void BackView()
	{
		await UIViewManager.Instance.BackView(this);
	}

	public override async UniTask Preopning(CancellationToken token)
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
