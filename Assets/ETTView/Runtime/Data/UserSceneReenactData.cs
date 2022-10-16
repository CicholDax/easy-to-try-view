using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETTView.Data;

[System.Serializable]
public class UserSceneReenactData : UserData
{
	//Reenactable�R���|�[�l���g���n���h������V���A���C�Y���ꂽ�����i�[
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

		public GameObjectReenactData(Reenactable reenactable)
		{
			_instanceId = reenactable.GetInstanceID();
			_name = reenactable.name;
			_active = reenactable.gameObject.activeSelf;
			_pos = reenactable.transform.position;
			_rot = reenactable.transform.rotation;
			_scl = reenactable.transform.localScale;

			//�A�^�b�`����Ă�MonoBehavior�����ׂĎ擾
			_monoBehaviors.Clear();
			var monos = reenactable.GetComponents<MonoBehaviour>();
			foreach(var mono in monos)
			{
				_monoBehaviors.Add(new MonoBehaviorReenactData(mono));
			}
		}

		public bool IsMatch(Reenactable reenactable)
		{
			return _instanceId == reenactable.GetInstanceID();
		}

		public void Reenact(Reenactable target)
		{
			target.name = _name;
			target.gameObject.SetActive(_active);

			//Tranceform���𕜌��iTranmsform��Mono����Ȃ��ăV���A���C�Y����Ȃ�����ʓr�Ή��j
			target.transform.position = _pos;
			target.transform.rotation = _rot;
			target.transform.localScale = _scl;

			//�A�^�b�`����Ă�MonoBehavior�����ׂĎ擾
			var targetMonos = target.GetComponents<MonoBehaviour>();

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

		//�Č��Ώۂ̃I�u�W�F�N�g�����X�g��
		var list = GameObject.FindObjectsOfType<Reenactable>();

		foreach(var target in list)
		{
			_dataList.Add(new GameObjectReenactData(target));
		}

		SaveToPrefs();
	}

	public void Load()
	{
		//DataList�𑖍����Ȃ���A���[�h�ς݂̃I�u�W�F�N�g�̃C���X�^���XID�Əƍ����Ĕ��f
		var list = GameObject.FindObjectsOfType<Reenactable>();

		foreach (var dest in list)
		{
			//�C���X�^���XID����v�����f�[�^��GameObject�ɔ��f
			var src = _dataList.Find((d) => d.IsMatch(dest));
			if (src != null)
			{
				src.Reenact(dest);
			}
		}
	}
}
