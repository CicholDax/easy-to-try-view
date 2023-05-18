using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETTView.Data
{
	public class UserDataManager : Singleton<UserDataManager>
	{
		public Dictionary<string, UserData> _loadedList = new Dictionary<string, UserData>();

		public T Get<T>() where T : UserData, new()
		{
			var key = typeof(T).Name;
			if (!_loadedList.ContainsKey(key))
			{
				//リモートの場合はAPIから取る
				var ret = UserData.LoadFromPrefs<T>();
				if (ret == null)
				{
					ret = new T();
					ret.Reset();
				};
				_loadedList.Add(key, ret);
			}

			return _loadedList[key] as T;
		}

		public void Delete<T>() where T : UserData, new()
		{ 
			var key = typeof(T).Name;
			UserData.Delete<T>();
			if (_loadedList.ContainsKey(key))
			{
				_loadedList.Remove(key);
			}
		}

		public void DeleteAll()
		{
			_loadedList = new Dictionary<string, UserData>();
		}

		public bool IsExist<T>() where T : UserData, new()
		{
			return _loadedList.ContainsKey(typeof(T).Name);
		}

		public void Reset<T>() where T : UserData, new()
		{
			var key = typeof(T).Name;
			if (_loadedList.ContainsKey(key))
			{
				_loadedList[key].Reset();
				_loadedList[key].SaveToPrefs();
			}
		}
	}
}