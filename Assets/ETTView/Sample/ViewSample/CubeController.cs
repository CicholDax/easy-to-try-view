using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETTView;
using ETTView.Data;
using Cysharp.Threading.Tasks;
using System.Threading;

public class CubeController : Reopenable
{

	[SerializeField] string _test;

	public override UniTask Opening(CancellationToken token)
	{
		return base.Opening(token);
	}


	public void Update()
	{
		if(Input.GetKeyDown(KeyCode.F1))
		{
			var userData = UserDataManager.Instance.Get<UserSceneReenactData>();
			userData.Save();
		}


		if (Input.GetKeyDown(KeyCode.F2))
		{
			var userData = UserDataManager.Instance.Get<UserSceneReenactData>();
			userData.Load();
		}


	}

}
