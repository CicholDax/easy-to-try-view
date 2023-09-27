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

			if (!string.IsNullOrEmpty(_prefabPath)) Debug.Log( "SAVED! : " +_prefabPath + ":" + _json);

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


			if (!string.IsNullOrEmpty(_prefabPath)) Debug.Log("LOAD! : " + _prefabPath + ":" + _json);

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
			if (string.IsNullOrEmpty(_prefabPath))
			{
				return null;
			}
			Debug.Log(_prefabPath + "Renact!!" + _uniId);
			var prefab = Resources.Load<Reenactable>(_prefabPath);
			Debug.Log("prefab transform " + prefab.transform.position + "/" +  prefab.transform.localScale + "/" + prefab.transform.localRotation);

			Debug.Log("data transform " + _pos + "/" + _scl + "/" + _rot);
			if (prefab == null) return null;

			var instance = Instantiate(prefab);
			Debug.Log("instance transform" + instance.transform.position + "/" + instance.transform.localScale + "/" + instance.transform.localRotation);
			Reenact(instance);
			Debug.Log("afterrenact transform" + instance.transform.position + "/" + instance.transform.localScale + "/" + instance.transform.localRotation);
			instance.OnDataLoadAfter(key);

			return instance;
		}

		[SerializeField] public string _uniId; //ユニークID
		[SerializeField] string _json;
		[SerializeField] bool _enable;
		[SerializeField] public string _prefabPath;

		//GameObject情報　複数アタッチの場合冗長になるが許容
		[SerializeField] public string _name;
		[SerializeField] public bool _active;
		[SerializeField] Vector3 _pos;
		[SerializeField] Quaternion _rot;
		[SerializeField] Vector3 _scl;
	}

	[SerializeField] string _uniId = Guid.NewGuid().ToString();
	[SerializeField] string _prefabPath;    //プレハブの場合
	[SerializeField] UnityEvent<string> _onDataLoad;
	[SerializeField] UnityEvent<string> _onDataSave;

	public void ResetUniId()
	{
#if UNITY_EDITOR
		_uniId = Guid.NewGuid().ToString();
#endif
	}

	public virtual void OnDataSaveBefore(string key)
	{
		_onDataSave?.Invoke(key);
	}

	public virtual void OnDataLoadAfter(string key)
	{
		_onDataLoad?.Invoke(key);
	}
}
