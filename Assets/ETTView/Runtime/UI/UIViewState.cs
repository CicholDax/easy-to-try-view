using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETTView.UI;
using ETTView;
using Cysharp.Threading.Tasks;
using System.Threading;

public class UIViewState : ReopnablePrefab
{
	[SerializeField] UIView _view;
	[SerializeField] bool _awaitCloseState = false;

	public UIView View
	{
		get
		{
			if (_view == null)
			{
				_view = GetComponentInParent<UIView>();
				if(_view == null)
				{
					_view = UIViewManager.Instance.Current;
				}
			}
			return _view;
		}
	}

	public bool AwaitCloseState => _awaitCloseState;

	public async void BackView()
	{
		await View.BackView(this);
	}

	public override async UniTask Preopning(CancellationToken token)
	{
		if (_awaitCloseState)
		{
			await View.RegistState(this);
		}
		else
		{
			View.RegistState(this).Forget();
		}
	}
}
