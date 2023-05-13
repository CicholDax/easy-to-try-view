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
}
