using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using ETTView.Data;
using System.Text.RegularExpressions;
using Unity.Plastic.Newtonsoft.Json;
using DG.DemiEditor;

public class Reenactable : MonoBehaviour
{
	[System.Serializable]
	public class Data
	{
		public Data(Reenactable reenactable)
		{
			_uniId = reenactable._uniId;

			_json = JsonUtility.ToJson(reenactable);

			_enable = reenactable.enabled;
			_name = reenactable.name;
			_active = reenactable.gameObject.activeSelf;
			_pos = reenactable.transform.localPosition;
			_rot = reenactable.transform.localRotation;
			_scl = reenactable.transform.localScale;
		}
		public void Reenact(Reenactable target)
		{
			target.enabled = _enable;

			//コールバックはシリアライズしたくない
			var onDataLoad = target._onDataLoad.Clone();
			var onDataSave = target._onDataSave.Clone();
			JsonUtility.FromJsonOverwrite(_json, target);
			target._onDataLoad = onDataLoad;
			target._onDataSave = onDataSave;

			target.gameObject.name = _name;
			target.gameObject.SetActive(_active);
			target.transform.localPosition = _pos;
			target.transform.localRotation = _rot;
			target.transform.localScale = _scl;
		}

		public bool IsMatch(Reenactable target)
		{
			return _uniId == target._uniId;
		}

		[SerializeField] string _uniId; //ユニークID
		[SerializeField] string _json;
		[SerializeField] bool _enable;

		//GameObject情報　複数アタッチの場合冗長になるが許容
		[SerializeField] string _name;
		[SerializeField] bool _active;
		[SerializeField] Vector3 _pos;
		[SerializeField] Quaternion _rot;
		[SerializeField] Vector3 _scl;
	}

	[SerializeField] string _uniId = Guid.NewGuid().ToString();
	[SerializeField] UnityEvent _onDataLoad;
	[SerializeField] UnityEvent _onDataSave;

	public virtual void OnDataSaveBefore(string key)
	{
		_onDataSave?.Invoke();
	}

	public virtual void OnDataLoadAfter(string key)
	{
		_onDataLoad?.Invoke();
	}
}
