using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETTView.Data
{
	public class MasterDataManager : Singleton<MasterDataManager>
	{
		Dictionary<string, IMasterData> _loadedList = new Dictionary<string, IMasterData>();

		public T Get<T>() where T : UnityEngine.Object, IMasterData
		{
			var key = typeof(T).Name;
			if (!_loadedList.ContainsKey(key))
			{
				var ret = Resources.Load<T>(key);
				_loadedList.Add(key, ret);
			}

			return _loadedList[key] as T;
		}
	}
}
