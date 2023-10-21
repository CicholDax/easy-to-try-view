using Cysharp.Threading.Tasks;
using ETTView.UI;
using ETTView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackableReopnablePrefab : ReopnablePrefab
{
	public void BackForget()
	{
		UIViewManager.Instance.Back(this).Forget();
	}
	public void BackForget(Reopnable target)
	{
		UIViewManager.Instance.Back(target).Forget();
	}

	public UniTask Back()
	{
		return UIViewManager.Instance.Back(this);
	}

	public UniTask Back(Reopnable target)
	{
		return UIViewManager.Instance.Back(target);
	}
}
