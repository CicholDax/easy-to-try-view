using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETTView.Data
{
	public class UserDataManager : Singleton<UserDataManager>
	{
		Dictionary<string, UserData> _loadedList = new Dictionary<string, UserData>();

        CustomPlayerPrefs _customPlayerPrefas;
        public CustomPlayerPrefs CustomPlayerPrefas
        {
            set
            {
                _customPlayerPrefas = value;
            }
            get
            {
                if(_customPlayerPrefas == null )
                {
                    _customPlayerPrefas = new CustomPlayerPrefs();
                }

                return _customPlayerPrefas;
            }
        }


        public class CustomPlayerPrefs
        {
			public virtual string GetString(string key) => PlayerPrefs.GetString(key);
            public virtual void SetString(string key, string value) => PlayerPrefs.SetString(key, value);
            public virtual bool HasKey(string key) => PlayerPrefs.HasKey(key);
            public virtual void DeleteKey(string key) => PlayerPrefs.DeleteKey(key);
            public virtual void Save() => PlayerPrefs.Save();
        }


		public T Get<T>() where T : UserData, new()
		{
			var key = typeof(T).Name;
			if (!_loadedList.ContainsKey(key))
			{
				var ret = LoadFromPrefs<T>();
				if (ret == null)
				{
					ret = new T();
				};
				_loadedList.Add(key, ret);
			}

			return _loadedList[key] as T;
		}

		public void Delete<T>() where T : UserData, new()
		{ 
			var key = typeof(T).Name;
			
			if (_loadedList.ContainsKey(key))
			{
				_loadedList.Remove(key);
			}
		}

        public bool IsExist<T>() where T : UserData, new()
        {
            return CustomPlayerPrefas.HasKey(typeof(T).Name);
        }

        public void SaveToPrefs(UserData userData)
        {
            var jsonText = JsonUtility.ToJson(userData);
            CustomPlayerPrefas.SetString(userData.GetType().Name, jsonText);
        }

        T LoadFromPrefs<T>() where T : UserData, new()
        {
            T ret = new T();
            var jsonText = CustomPlayerPrefas.GetString(typeof(T).Name);
            if (jsonText != null)
            {
                ret = JsonUtility.FromJson<T>(jsonText);
            }
            return ret;
        }
    }
}