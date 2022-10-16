using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETTView.Data;

[System.Serializable]
public class UserSceneReenactData : UserData
{
	//Reenactableコンポーネントがハンドルするシリアライズされた情報を格納
	[System.Serializable]
	class GameObjectReenactData
	{
		[System.Serializable]
		class MonoBehaviorReenactData
		{
			[SerializeField] int _instanceId;
			[SerializeField] string _json;
			[SerializeField] bool _enable;

			public MonoBehaviorReenactData(MonoBehaviour mono)
			{
				_instanceId = mono.GetInstanceID();
				_enable = mono.enabled;
				_json = JsonUtility.ToJson(mono);
			}

			public bool IsMatch(MonoBehaviour mono)
			{
				return _instanceId == mono.GetInstanceID();
			}

			public void Reenact(MonoBehaviour target)
			{
				target.enabled = _enable;
				JsonUtility.FromJsonOverwrite(_json, target);
			}
		}

		[SerializeField] int _instanceId;
		[SerializeField] string _name;
		[SerializeField] bool _active;
		[SerializeField] Vector3 _pos;
		[SerializeField] Quaternion _rot;
		[SerializeField] Vector3 _scl;

		[SerializeField] List<MonoBehaviorReenactData> _monoBehaviors = new List<MonoBehaviorReenactData>();

		public GameObjectReenactData(GameObject go)
		{
			_instanceId = go.GetInstanceID();
			_name = go.name;
			_active = go.gameObject.activeSelf;
			_pos = go.transform.position;
			_rot = go.transform.rotation;
			_scl = go.transform.localScale;

			//アタッチされてるMonoBehaviorをすべて取得
			_monoBehaviors.Clear();
			var monos = go.GetComponents<MonoBehaviour>();
			foreach(var mono in monos)
			{
				_monoBehaviors.Add(new MonoBehaviorReenactData(mono));
			}
		}

		public bool IsMatch(GameObject go)
		{
			return _instanceId == go.GetInstanceID();
		}

		public void Reenact(GameObject go)
		{
			go.name = _name;
			go.gameObject.SetActive(_active);

			//Tranceform情報を復元（TranmsformはMonoじゃなくてシリアライズされないから別途対応）
			go.transform.position = _pos;
			go.transform.rotation = _rot;
			go.transform.localScale = _scl;

			//アタッチされてるMonoBehaviorをすべて取得
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

	[SerializeField] List<GameObjectReenactData> _dataList = new List<GameObjectReenactData>();


	public void Save()
	{
		_dataList.Clear();

		//再現対象のオブジェクトをリストに
		var targets = GameObject.FindObjectsOfType<Reenactable>();

		void GetDataList( List<GameObjectReenactData> list, GameObject go, bool isRecursive)
		{
			list.Add(new GameObjectReenactData(go));

			if (isRecursive)
			{
				foreach (Transform child in go.transform)
				{
					GetDataList(list, child.gameObject, isRecursive);
				}
			}
		};

		foreach(var target in targets)
		{
			GetDataList(_dataList, target.gameObject, target.IsRecursive);
		}

		SaveToPrefs();
	}

	public void Load()
	{
		//DataListを走査しながら、ロード済みのオブジェクトのインスタンスIDと照合して反映
		var list = GameObject.FindObjectsOfType<GameObject>();

		//再現データを走査して
		foreach(var data in _dataList)
		{
			//インスタンスIDが一致したら反映
			var desc = list.ToList().Find((d) => data.IsMatch(d));
			if(desc != null)
			{
				data.Reenact(desc);
			}
		}
	}
}
