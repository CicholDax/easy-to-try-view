using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETTView.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class AView : UIView
{
	public override async UniTask OnLoadedUpdate()
	{
		if(Input.GetKeyDown(KeyCode.D))
		{
			var popup = await Popup.CreateFromResources<APopup>();
		}

		if(Input.GetKeyDown(KeyCode.F1))
		{
			await SceneManager.LoadSceneAsync("BView");
		}
	}
}
