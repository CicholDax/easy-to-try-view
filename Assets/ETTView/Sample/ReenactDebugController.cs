using ETTView.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReenactDebugController : MonoBehaviour
{
    [SerializeField] GameObject _cubePrefab;

	GameObject _ins;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
			_ins = Instantiate(_cubePrefab);
        }

		if(Input.GetKeyDown(KeyCode.F2))
		{
			_ins.transform.localScale = Vector3.one * 3 * Random.value;

			_ins.transform.localPosition = Vector3.one * 1 * Random.value;

		}

		if (Input.GetKeyDown(KeyCode.F3))
		{
			UserDataManager.Instance.Get<UserSceneReenactData>().Save();
		}


		if (Input.GetKeyDown(KeyCode.F4))
		{
			SceneManager.LoadScene("ReenactDebug");
		}

		if (Input.GetKeyDown(KeyCode.F5))
		{
			UserDataManager.Instance.Get<UserSceneReenactData>().Load();
		}
	}
}
