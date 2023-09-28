using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETTView.Data;
using System.Runtime.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class UserSceneReenactData : UserData
{
	[System.Serializable]
	class DataSet
	{
		[SerializeField] List<Reenactable.Data> _dataList = new List<Reenactable.Data>();

		public List<Reenactable.Data> DataList => _dataList;
	}

	[SerializeField] List<string> _dataKeys = new List<string>();
	[SerializeField] List<DataSet> _dataSetList = new List<DataSet>();

	List<Reenactable.Data> GetOrCreateData(string key)
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
		GetOrCreateData(key).Clear();

		// �S�Ẵ��[�g�Q�[���I�u�W�F�N�g���擾
		var allRootGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
		List<Reenactable> targets = new List<Reenactable>();

		foreach (var rootGameObject in allRootGameObjects)
		{
			// �A�N�e�B�u�E��A�N�e�B�u�ȃI�u�W�F�N�g���܂߂� Reenactable �R���|�[�l���g������
			var reenactables = rootGameObject.GetComponentsInChildren<Reenactable>(true);
			targets.AddRange(reenactables);
		}

		foreach (var target in targets)
		{
			target.OnDataSaveBefore(key);
			GetOrCreateData(key).Add(new Reenactable.Data(target));
		}

		UserDataManager.Instance.SaveToPrefs(this);
	}

	public void Load(string key = "")
	{
		//DataList�𑖍����Ȃ���A���[�h�ς݂̃I�u�W�F�N�g�̃C���X�^���XID�Əƍ����Ĕ��f
		List<Reenactable> list = Resources.FindObjectsOfTypeAll<Reenactable>().ToList();

		//�Č��f�[�^�𑖍�����
		foreach(var data in GetOrCreateData(key))
		{
			//�C���X�^���XID����v�����甽�f
			var desc = list.ToList().Find((d) => data.IsMatch(d));
			if (desc != null)
			{
				data.Reenact(desc);
				desc.OnDataLoadAfter(key);
			}
			else
			{
				//Debug.Log("Missing ReenactableData " + data._uniId + "/" + data._name + "/" + data._prefabPath + "/" + data._active);

				//��v����C���X�^���XID���Ȃ��Ă���Prefab����ێ����Ă��琶�����ĕ���
				data.InstantiateAndReenactIfPathExists(key);
			}
			list.Remove(desc);
		}
	}
}
