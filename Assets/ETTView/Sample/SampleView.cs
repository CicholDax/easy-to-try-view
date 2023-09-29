using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETTView;
using ETTView.UI;
using UnityEngine.SceneManagement;

public class SampleView : UIView
{
	public void OnClickViewSample()
	{
		SceneManager.LoadSceneAsync("ViewSampleScene", LoadSceneMode.Additive);
	}

	public void OnClickReenactableSample()
	{
        SceneManager.LoadSceneAsync("ReenactSampleScene", LoadSceneMode.Additive);
    }

	public async void OnClickShowPopup()
	{
		await CommonPopup.Create(transform, "確認", "ポップアップだよーん");
	}
}
