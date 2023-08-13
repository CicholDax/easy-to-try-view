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

				var type = typeof(T);

				lock (_lock)
				{
					if (_instance == null)
					{
						//�����v���n�u��Resource�ɂ��邩�m�F
						var prefab = Resources.Load<T>(type.Name);
						if (prefab != null)
						{
							//�v���n�u���w�肳��Ă��琶��
							_instance = Instantiate(prefab);
						}
						else
						{
							var go = new GameObject(type.Name, type);
							_instance = go.GetComponent<T>();
						}

						if (_instance.IsDontDestroy)
						{
							DontDestroyOnLoad(_instance.gameObject);
						}
					}
				}

				return _instance;
			}
		}
	}
}