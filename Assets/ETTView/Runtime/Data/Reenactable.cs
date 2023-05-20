using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using ETTView.Data;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Reenactable : MonoBehaviour
{
	[System.Serializable]
	public class Data
	{
		public Data(Reenactable reenactable)
		{
			_uniId = reenactable._uniId;
			_prefabPath = reenactable._prefabPath;

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
			//var onDataLoad = target._onDataLoad.Clone();
			//var onDataSave = target._onDataSave.Clone();
			JsonUtility.FromJsonOverwrite(_json, target);
			//target._onDataLoad = onDataLoad;
			//target._onDataSave = onDataSave;

			target.gameObject.name = _name;
			target.gameObject.SetActive(_active);
			target.transform.localPosition = _pos;
			target.transform.localRotation = _rot;
			target.transform.localScale = _scl;
		}

		public bool IsMatch(Reenactable target)
		{
			return
#if UNITY_EDITOR
			!EditorUtility.IsPersistent(target) && 
#endif
			_uniId == target._uniId;
		}

		//パスを保持していたら生成して復元
		public Reenactable InstantiateAndReenactIfPathExists(string key)
		{
			if (string.IsNullOrEmpty(_prefabPath)) return null;
			var prefab = Resources.Load<Reenactable>(_prefabPath);
			if (prefab == null) return null;
			var instance = Instantiate(prefab);
			Reenact(instance);
			instance.OnDataLoadAfter(key);
			return instance;
		}

		[SerializeField] string _uniId; //ユニークID
		[SerializeField] string _json;
		[SerializeField] bool _enable;
		[SerializeField] string _prefabPath;

		//GameObject情報　複数アタッチの場合冗長になるが許容
		[SerializeField] string _name;
		[SerializeField] bool _active;
		[SerializeField] Vector3 _pos;
		[SerializeField] Quaternion _rot;
		[SerializeField] Vector3 _scl;
	}

	[SerializeField] string _uniId = Guid.NewGuid().ToString();
	[SerializeField] string _prefabPath;    //プレハブの場合
	[SerializeField] UnityEvent<string> _onDataLoad;
	[SerializeField] UnityEvent<string> _onDataSave;

	public virtual void OnDataSaveBefore(string key)
	{
		_onDataSave?.Invoke(key);
	}

	public virtual void OnDataLoadAfter(string key)
	{
		_onDataLoad?.Invoke(key);
	}
}
