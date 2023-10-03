using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETTView.Data;
using UnityEngine.SceneManagement;

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
	[SerializeField] public string _debugText;

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

#if UNITY_EDITOR
		List<Reenactable> targets = new List<Reenactable>();

		int sceneCount = 0;
		int rootCount = 0;

		for (int i = 0; i < SceneManager.sceneCount; i++)
		{
			Scene scene = SceneManager.GetSceneAt(i);
			sceneCount++;

			foreach (var rootGameObject in scene.GetRootGameObjects())
			{
				rootCount++;
				targets.AddRange(rootGameObject.GetComponentsInChildren<Reenactable>(true));
			}
		}
		_debugText = "targets:" + targets.Count + "/" + "sceneCount:" + sceneCount + "/" + "rootCount:" + rootCount;

#else
		var targets = Resources.FindObjectsOfTypeAll<Reenactable>();
		_debugText = "targets:" + targets.Count();
#endif

		foreach (var target in targets)
		{
			target.OnDataSaveBefore(key);
			GetOrCreateData(key).Add(new Reenactable.Data(target));
		}

		UserDataManager.Instance.SaveToPrefs(this);
	}

	public void Load(string key = "")
	{
		// �S�Ẵ��[�g�Q�[���I�u�W�F�N�g���擾
#if UNITY_EDITOR
		List<Reenactable> targets = new List<Reenactable>();

		int sceneCount = 0;
		int rootCount = 0;

		for (int i = 0; i < SceneManager.sceneCount; i++)
		{
			Scene scene = SceneManager.GetSceneAt(i);
			sceneCount++;

			foreach (var rootGameObject in scene.GetRootGameObjects())
			{
				rootCount++;
				targets.AddRange(rootGameObject.GetComponentsInChildren<Reenactable>(true));
			}
		}
#else
		var targets = Resources.FindObjectsOfTypeAll<Reenactable>();
#endif

		//�Č��f�[�^�𑖍�����
		foreach (var data in GetOrCreateData(key))
		{
			//�C���X�^���XID����v�����甽�f
			var desc = targets.Find((d) => data.IsMatch(d));
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
			targets.Remove(desc);
		}
	}
}
