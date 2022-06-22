using UnityEngine;

namespace ETTView
{
	public interface ISingletonMono
    {
		public bool IsDontDestroy { get; }
    }

	public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>, ISingletonMono
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

				if (_instance.IsDontDestroy)
				{
					DontDestroyOnLoad(go);
				}

				_instance = go.GetComponent<T>();

				return _instance;
			}
		}
	}
}