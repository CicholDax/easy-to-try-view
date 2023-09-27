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

			//�R�[���o�b�N�̓V���A���C�Y�������Ȃ�
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

		//�p�X��ێ����Ă����琶�����ĕ���
		public Reenactable InstantiateAndReenactIfPathExists(string key)
		{
			if (string.IsNullOrEmpty(_prefabPath))
			{
				Debug.Log("InstantiateAndReenactIfPathExists 1" + _prefabPath);
				return null;
			}
			var prefab = Resources.Load<Reenactable>(_prefabPath);

			Debug.Log("InstantiateAndReenactIfPathExists 2" + prefab);

			Debug.Log("transform " + prefab.transform.position + "/" +  prefab.transform.localScale + "/" + prefab.transform.localRotation);

			if (prefab == null) return null;


			foreach (Transform child in prefab.transform)
			{
				Debug.Log(prefab.name + child.name + ":" + child.gameObject.activeInHierarchy);
			}

			Debug.Log("InstantiateAndReenactIfPathExists 3" + prefab.gameObject.activeInHierarchy);

			var instance = Instantiate(prefab);


			Debug.Log("transform 1" + instance.transform.position + "/" + instance.transform.localScale + "/" + instance.transform.localRotation);


			foreach (Transform child in instance.transform)
			{
				Debug.Log(prefab.name + child.name + ":" + child.gameObject.activeInHierarchy);
			}

			Debug.Log("InstantiateAndReenactIfPathExists 4" + instance.gameObject.activeInHierarchy);
			instance.gameObject.SetActive(true);


			Debug.Log("InstantiateAndReenactIfPathExists 5" + instance.gameObject.activeInHierarchy);
			Reenact(instance);


			Debug.Log("transform 2" + instance.transform.position + "/" + instance.transform.localScale + "/" + instance.transform.localRotation);

			Debug.Log("InstantiateAndReenactIfPathExists 6" + instance.gameObject.activeInHierarchy);
			instance.OnDataLoadAfter(key);


			Debug.Log("transform 3" + instance.transform.position + "/" + instance.transform.localScale + "/" + instance.transform.localRotation);


			Debug.Log("InstantiateAndReenactIfPathExists 7" + instance.gameObject.activeInHierarchy);
			return instance;
		}

		[SerializeField] public string _uniId; //���j�[�NID
		[SerializeField] string _json;
		[SerializeField] bool _enable;
		[SerializeField] public string _prefabPath;

		//GameObject���@�����A�^�b�`�̏ꍇ�璷�ɂȂ邪���e
		[SerializeField] public string _name;
		[SerializeField] public bool _active;
		[SerializeField] Vector3 _pos;
		[SerializeField] Quaternion _rot;
		[SerializeField] Vector3 _scl;
	}

	[SerializeField] string _uniId = Guid.NewGuid().ToString();
	[SerializeField] string _prefabPath;    //�v���n�u�̏ꍇ
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
