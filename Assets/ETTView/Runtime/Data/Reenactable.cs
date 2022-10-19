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

	public virtual void OnDataSave()
	{
		_onDataSave?.Invoke();
	}

	public virtual void OnDataLoad()
	{
		_onDataLoad?.Invoke();
	}
}
