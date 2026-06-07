using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETTView.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class AView : UIView
{
	public async void Update()
	{

		if(Input.GetKeyDown(KeyCode.F1))
		{
			await SceneManager.LoadSceneAsync("BView");
		}
	}
}
