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

		/// <summary>
		/// インスタンスを返す。存在しない場合は自動生成する。
		/// レンダリングコールバック内では Awake が遅延するため呼ばないこと。
		/// </summary>
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
							_instance = Instantiate(prefab);
						else
						{
							var go = new GameObject(typeof(T).Name);
							_instance = go.AddComponent<T>();
						}
					}
				}

				return _instance;
			}
		}

		/// <summary>
		/// インスタンスが既に存在する場合のみ返す。自動生成しない。
		/// レンダリングコールバックなど Awake が遅延する文脈ではこちらを使う。
		/// </summary>
		public static T InstanceOrNull => _instance;

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
