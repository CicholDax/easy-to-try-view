using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETTView.Data;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class UserSceneReenactData : UserData
{
	static public string GetHierarchyPath(MonoBehaviour target)
	{
		string path = "";
		Transform current = target.transform;
		while (current != null)
		{
			// �����K�w�ɓ����̃I�u�W�F�N�g������ꍇ������̂ŁA������������
			int index = current.GetSiblingIndex();
			path = "/" + current.name + index + path;
			current = current.parent;
		}

		return "/" + target.gameObject.scene.name + path + "/" + target.GetType().Name;
	}

	static public string GetHierarchyPath(GameObject target)
	{
		string path = "";
		Transform current = target.transform;
		while (current != null)
		{
			// �����K�w�ɓ����̃I�u�W�F�N�g������ꍇ������̂ŁA������������
			int index = current.GetSiblingIndex();
			path = "/" + current.name + index + path;
			current = current.parent;
		}

		return "/" + target.gameObject.scene.name + path + "/" + target.GetType().Name;
	}

	//Reenactable�R���|�[�l���g���n���h������V���A���C�Y���ꂽ�����i�[
	[System.Serializable]
	class GameObjectReenactData
	{
		[System.Serializable]
		class MonoBehaviorReenactData
		{
			[SerializeField] string _uniId;	//���j�[�NID
			[SerializeField] string _json;
			[SerializeField] bool _enable;

			public MonoBehaviorReenactData(MonoBehaviour mono)
			{
				_uniId = GetHierarchyPath(mono);
				_enable = mono.enabled;
				_json = JsonUtility.ToJson(mono);
			}

			public bool IsMatch(MonoBehaviour mono)
			{
				return _uniId == GetHierarchyPath(mono);
			}

			public void Reenact(MonoBehaviour target)
			{
				target.enabled = _enable;
				JsonUtility.FromJsonOverwrite(_json, target);
			}
		}

		[SerializeField] string _uniId;
		[SerializeField] string _name;
		[SerializeField] bool _active;
		[SerializeField] Vector3 _pos;
		[SerializeField] Quaternion _rot;
		[SerializeField] Vector3 _scl;
		[SerializeField] string _prefabPath;

		[SerializeField] List<MonoBehaviorReenactData> _monoBehaviors = new List<MonoBehaviorReenactData>();

		public GameObjectReenactData(GameObject go, bool isRecursive)
		{
			_uniId = GetHierarchyPath(go);
			_name = go.name;
			_active = go.gameObject.activeSelf;
			_pos = go.transform.position;
			_rot = go.transform.rotation;
			_scl = go.transform.localScale;

			//�v���n�u����ێ����Ă��炻����ۑ�
			var prefabData = go.GetComponent<ReenactablePrefab>();
			if(prefabData != null)
			{
				_prefabPath = prefabData.Path;
			}

			//�A�^�b�`����Ă�MonoBehavior�����ׂĎ擾
			if (isRecursive)
			{
				_monoBehaviors.Clear();
				var monos = go.GetComponents<MonoBehaviour>();
				foreach (var mono in monos)
				{
					if (mono != null)
						_monoBehaviors.Add(new MonoBehaviorReenactData(mono));
				}
			}
			else
			{
				//�ċA�I�ݒ肶��Ȃ�������Reenactable�̂�
				var monos = go.GetComponents<Reenactable>();
				foreach (var mono in monos)
				{
					if (mono != null)
						_monoBehaviors.Add(new MonoBehaviorReenactData(mono));
				}
			}
		}

		public bool IsMatch(GameObject go)
		{
			return _uniId == GetHierarchyPath(go);
		}

		public void ReenactPrefab()
		{
			var prefab = Object.Instantiate(Resources.Load<ReenactablePrefab>(_prefabPath));
			Reenact(prefab.gameObject);
		}

		public void Reenact(GameObject go)
		{
			go.name = _name;
			go.gameObject.SetActive(_active);

			//Tranceform���𕜌��iTranmsform��Mono����Ȃ��ăV���A���C�Y����Ȃ�����ʓr�Ή��j
			go.transform.position = _pos;
			go.transform.rotation = _rot;
			go.transform.localScale = _scl;

			//�A�^�b�`����Ă�MonoBehavior�����ׂĎ擾
			var targetMonos = go.GetComponents<MonoBehaviour>();

			foreach (var dest in targetMonos)
			{
				var src = _monoBehaviors.Find((d) => d.IsMatch(dest));
				if (src != null)
				{
					src.Reenact(dest);
				}
			}
		}
	}

	[System.Serializable]
	class DataSet
	{
		[SerializeField] List<GameObjectReenactData> _dataList = new List<GameObjectReenactData>();
		public List<GameObjectReenactData> DataList => _dataList;
	}

	[SerializeField] List<string> _dataKeys = new List<string>();
	[SerializeField] List<DataSet> _dataSetList = new List<DataSet>();

	List<GameObjectReenactData> GetDataValueList(string key)
	{
		var index = _dataKeys.FindLastIndex((d) => d == key);
		if (index < 0)
		{
			_dataKeys.Add(key);	//�Ȃ�������ǉ�
			index = _dataKeys.Count-1;
		}

		if(_dataSetList.Count <= index )
		{
			_dataSetList.Add(new DataSet());
		}

		return _dataSetList[index].DataList;
	}

	public void Save(string key = "")
	{
		GetDataValueList(key).Clear();

		//�Č��Ώۂ̃I�u�W�F�N�g�����X�g��
		var targets = Resources.FindObjectsOfTypeAll<Reenactable>();

		void GetDataList(List<GameObjectReenactData> list, GameObject go, bool isRecursive)
		{
			list.Add(new GameObjectReenactData(go, isRecursive));

			if (isRecursive)
			{
				foreach (Transform child in go.transform)
				{
					GetDataList(list, child.gameObject, isRecursive);
				}
			}
		};

		foreach (var target in targets)
		{

			//Editgor���ƃv���n�u�t�@�C�����������Ⴄ�݂����Ȃ̂ŏ��O
#if UNITY_EDITOR
			if (EditorUtility.IsPersistent(target)) continue;
#endif
			target.OnDataSave();
			GetDataList(GetDataValueList(key), target.gameObject, target.IsRecursive);
		}

		SaveToPrefs();
	}

	public void Load(string key = "")
	{
		//DataList�𑖍����Ȃ���A���[�h�ς݂̃I�u�W�F�N�g�̃C���X�^���XID�Əƍ����Ĕ��f
		var list = Resources.FindObjectsOfTypeAll<GameObject>();

		//�Č��f�[�^�𑖍�����
		foreach(var data in GetDataValueList(key))
		{
			//�C���X�^���XID����v�����甽�f
			var desc = list.ToList().Find((d) => data.IsMatch(d));
			if(desc != null)
			{
				data.Reenact(desc);

				var reenactable = desc.GetComponent<Reenactable>();
				if (reenactable != null)
				{
					reenactable.OnDataLoad();
				}
			}
			else
			{
				//��v����ID���Ȃ�������
				data.ReenactPrefab();
			}
		}
	}
}
