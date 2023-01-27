using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ETTView.Data;

public class Reenactable : MonoBehaviour
{
	[SerializeField] bool _isRecursive;
	[SerializeField] UnityEvent _onDataLoad;
	[SerializeField] UnityEvent _onDataSave;

	public bool IsRecursive => _isRecursive;

	public virtual void OnDataSave(string key)
	{
		_onDataSave?.Invoke();
	}

	public virtual void OnDataLoad(string key)
	{
		_onDataLoad?.Invoke();
	}
}
