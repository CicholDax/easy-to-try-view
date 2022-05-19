using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniDax.Data
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

		public void SaveToPrefs()
		{
			var jsonText = JsonUtility.ToJson(this);
			PlayerPrefs.SetString(this.GetType().Name, jsonText);
		}

	}
}