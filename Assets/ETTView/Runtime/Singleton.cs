namespace ETTView
{
	public class Singleton<T> where T : class, new()
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
						_instance = new T();
				}

				return _instance;
			}
		}
	}
}