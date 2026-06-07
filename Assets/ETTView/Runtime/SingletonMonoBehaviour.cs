using UnityEngine;

namespace ETTView
{
	public interface ISingletonMono
    {
		public bool IsDontDestroy { get; }
    }

	public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>, ISingletonMono
	{
        static readonly object _lock = new object();
        static T _instance = null;

		public static T Instance
		{
			get
			{
				if (_instance != null)
				{
					return _instance;
				}

				lock (_lock)
				{
					if (_instance == null)
					{
						var prefab = Resources.Load<T>(typeof(T).Name);
						if (prefab != null)
							Instantiate(prefab);
						else
							new GameObject(typeof(T).Name, typeof(T));
						// _instance は Awake でセットされる
					}
				}

				return _instance;
			}
		}

		protected void Awake()
		{
			if (_instance != null && _instance != this as T)
			{
				Destroy(gameObject);
				return;
			}

			_instance = this as T;

			if (_instance.IsDontDestroy)
			{
				DontDestroyOnLoad(gameObject);
			}

			OnAwake();
		}

		/// <summary>
		/// Awakeの代わりに派生クラスでオーバーライドする。
		/// </summary>
		protected virtual void OnAwake() { }
	}
}
