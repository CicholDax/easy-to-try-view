using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETTView.Data
{

	[System.Serializable]
	public class UserData
	{
		public virtual void Reset()
		{
			
		}

		public static T LoadFromPrefs<T>() where T : UserData, new()
		{
			T ret = new T();
			var jsonText = PlayerPrefs.GetString(typeof(T).Name);
			if (jsonText != null)
			{
				ret = JsonUtility.FromJson<T>(jsonText);
			}
			return ret;
		}

		public static UserData LoadFromPrefs(Type type, Func<UserData> createDefault)
		{
			UserData ret = null;
			var jsonText = PlayerPrefs.GetString(type.Name);
			if (jsonText != null)
			{
				ret = JsonUtility.FromJson(jsonText, type) as UserData;
			}

			if (ret == null) ret = createDefault.Invoke();

			return ret;
		}

		public void SaveToPrefs()
		{
			var jsonText = JsonUtility.ToJson(this);
			PlayerPrefs.SetString(this.GetType().Name, jsonText);
		}

	}
}