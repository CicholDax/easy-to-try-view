using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETTView;
using ETTView.Data;
using Cysharp.Threading.Tasks;

public class CubeController : Reopnable
{

	[SerializeField] string _test;

	public override UniTask Opening()
	{
		return base.Opening();
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
