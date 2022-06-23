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
		
		public static T LoadFromPrefs<T>() where T : UserData
		{
			T ret = default(T);
			var jsonText = PlayerPrefs.GetString(typeof(T).Name);
			if (jsonText != null)
			{
				ret = JsonUtility.FromJson<T>(jsonText);
			}
			return ret;
		}

		public static UserData LoadFromPrefs(Type type)
		{
			UserData ret = default(UserData);
			var jsonText = PlayerPrefs.GetString(type.Name);
			if (jsonText != null)
			{
				ret = JsonUtility.FromJson(jsonText, type) as UserData;
			}
			return ret;
		}

		public void SaveToPrefs()
		{
			var jsonText = JsonUtility.ToJson(this);
			PlayerPrefs.SetString(this.GetType().Name, jsonText);
		}

	}
}