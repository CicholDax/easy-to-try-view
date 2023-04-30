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

		//再現対象のオブジェクトをリストに
		var targets = Resources.FindObjectsOfTypeAll<Reenactable>();

		foreach (var target in targets)
		{
			//Editgorだとプレハブファイルも入っちゃうみたいなので除外
#if UNITY_EDITOR
			if (EditorUtility.IsPersistent(target)) continue;
#endif
			target.OnDataSaveBefore(key);
			GetOrCreateData(key).Add(new Reenactable.Data(target));
		}

		SaveToPrefs();
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
			if(desc != null)
			{
				data.Reenact(desc);
				desc.OnDataLoadAfter(key);
			}
			list.Remove(desc);
		}
	}
}
