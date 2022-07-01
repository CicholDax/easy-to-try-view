using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETTView.Data
{
	public interface IMasterData
	{

	}

	[System.Serializable]
	public abstract class MasterData<T> : ScriptableObject, IMasterData
	{
		//リモートの場合ここでDBアクセスして作ってAB配信したりする
		public abstract List<T> CreateDataList();

		public void Awake()
		{
			_dataList = CreateDataList();
		}

		public List<T> DataList { get { return _dataList; } }
		[SerializeField] protected List<T> _dataList;
	}
}