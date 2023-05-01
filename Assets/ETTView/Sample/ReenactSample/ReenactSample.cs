using ETTView.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReenactSample : MonoBehaviour
{
	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.F1))
		{
			UserDataManager.Instance.Get<UserSceneReenactData>().Save();
		}

		if (Input.GetKeyDown(KeyCode.F2))
		{
			UserDataManager.Instance.Get<UserSceneReenactData>().Load();
		}


		if (Input.GetKeyDown(KeyCode.F3))
		{
			UserDataManager.Instance.Get<UserSceneReenactData>().Save("a");
		}

		if (Input.GetKeyDown(KeyCode.F4))
		{
			UserDataManager.Instance.Get<UserSceneReenactData>().Load("a");
		}
	}
}
