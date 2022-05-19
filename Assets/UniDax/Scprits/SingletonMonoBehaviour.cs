using UnityEngine;

namespace UniDax
{
	public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
	{
		static T _instance = null;

		public static T Instance
		{
			get
			{
				if (_instance != null)
				{
					return _instance;
				}

				var type = typeof(T);
				var go = new GameObject(type.Name, type);
				DontDestroyOnLoad(go);

				_instance = go.GetComponent<T>();

				return _instance;
			}
		}
	}
}