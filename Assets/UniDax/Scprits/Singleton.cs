namespace UniDax
{
	public class Singleton<T> where T : class, new()
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

				_instance = new T();

				return _instance;
			}
		}
	}
}