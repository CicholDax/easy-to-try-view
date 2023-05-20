using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETTView.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class BView : UIView
{
	public async void Update()
	{
		if (Phase == ETTView.Reopener.PhaseType.Loading) return;

		if (Input.GetKeyDown(KeyCode.F1))
		{
			await SceneManager.LoadSceneAsync("AView");
		}
	}
}
