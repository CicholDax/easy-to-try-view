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
			_dataKeys.Add(key);	//なかったら追加
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

		// 全てのルートゲームオブジェクトを取得
		var allRootGameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
		List<Reenactable> targets = new List<Reenactable>();

		foreach (var rootGameObject in allRootGameObjects)
		{
			// アクティブ・非アクティブなオブジェクトも含めて Reenactable コンポーネントを検索
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
		//DataListを走査しながら、ロード済みのオブジェクトのインスタンスIDと照合して反映
		List<Reenactable> list = Resources.FindObjectsOfTypeAll<Reenactable>().ToList();

		//再現データを走査して
		foreach(var data in GetOrCreateData(key))
		{
			//インスタンスIDが一致したら反映
			var desc = list.ToList().Find((d) => data.IsMatch(d));
			if (desc != null)
			{
				data.Reenact(desc);
				desc.OnDataLoadAfter(key);
			}
			else
			{
				//Debug.Log("Missing ReenactableData " + data._uniId + "/" + data._name + "/" + data._prefabPath + "/" + data._active);

				//一致するインスタンスIDがなくてかつPrefab情報を保持してたら生成して復元
				data.InstantiateAndReenactIfPathExists(key);
			}
			list.Remove(desc);
		}
	}
}
