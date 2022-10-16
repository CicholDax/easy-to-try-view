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

	public override async UniTask OnLoadedUpdate()
	{
		Debug.Log("���͑҂�");
		await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.D));

		Debug.Log("�L�[�{�[�h�����͂���܂����I");
	}

	public void Update()
	{
		if(Input.GetKeyDown(KeyCode.D))
		{
			var userData = UserDataManager.Instance.Get<UserSceneReenactData>();
			userData.Save();
		}


		if (Input.GetKeyDown(KeyCode.E))
		{
			var userData = UserDataManager.Instance.Get<UserSceneReenactData>();
			userData.Load();
		}
	}

}