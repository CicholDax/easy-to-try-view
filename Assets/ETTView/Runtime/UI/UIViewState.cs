using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETTView.UI;
using ETTView;
using Cysharp.Threading.Tasks;

public class UIViewState : ReopnablePrefab
{
	public override async UniTask Preopning()
	{
		await UIViewManager.Instance.Current.RegistState(this);
	}
}
