using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestReenactable : Reenactable
{
	[SerializeField] string _textField;
	[SerializeField] GameObject _testObjectField;

	string _test;
	GameObject _testgo;


	public void Awake()
	{
		_test = "hoge";
		_testgo = _testObjectField;
	}

	public void Update()
	{
		if(Input.GetKeyDown(KeyCode.F3))
		{
			Debug.Log(_testgo.name);
		}
	}
}
