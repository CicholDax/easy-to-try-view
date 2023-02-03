using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

namespace ETTView
{
	public static class StateTypeEx
    {
		public static async UniTask WaitUntil(this Reopener.StateType now, Reopener.StateType state)
        {
			await UniTask.WaitUntil(() => now >= state);
		}
    }
	[DisallowMultipleComponent]
	public class Reopener : MonoBehaviour
	{
		public enum StateType
		{
			Loading,    //生成時に一度だけ
			Loaded,
			Preopening,
			Opening,
			Opened,
			Closing,
			Closed,
		}

		[SerializeField] UnityEvent _onLoaded;
		[SerializeField] UnityEvent _onOpen;
		[SerializeField] UnityEvent _onClose;

		public StateType State { get; private set; } = StateType.Loading;

		Reopnable[] _reopnables;
		Reopnable[] Reopnables
		{
			get
			{
				if (_reopnables == null)
				{
					_reopnables = GetComponents<Reopnable>();
				}
				return _reopnables;
			}
		}

		public async UniTask Open()
		{
			if (this == null) return;
			if (enabled) return;
			enabled = true;

			await UniTask.WaitUntil(() => State == StateType.Opened);
		}

		public async UniTask Close()
		{
			//ロード中だったら待つ
			await UniTask.WaitWhile(() => State == StateType.Loading);
			if (this == null) return;
			if (!enabled) return;
			enabled = false;

			await UniTask.WaitUntil(() => State == StateType.Closed);
		}


		protected virtual async void Awake()
		{
			UniTaskScheduler.UnobservedExceptionWriteLogType = LogType.Warning;

			await UniTask.Create(async () =>
			{
				await Load();
				State = StateType.Loaded;
			});
		}

		protected virtual async void OnEnable()
		{
			await UniTask.Create(async () =>
			{
				//ロード完了まで待つ
				await UniTask.WaitWhile(() => State == StateType.Loading);

				_onLoaded?.Invoke();

				State = StateType.Preopening;

				await Preopning();

				State = StateType.Opening;

				_onOpen?.Invoke();

				await Opening();

				State = StateType.Opened;

				await UniTask.WaitWhile(() => this != null && enabled);

				State = StateType.Closing;

				_onClose?.Invoke();

				await Closing();

				State = StateType.Closed;

			});
		}

		public async void Update()
		{
			if (State != StateType.Loading)
				await OnLoadedUpdate();
		}

		async UniTask Load()
		{
			if (this == null) return;
			Debug.Log(name + " Load Start");

			var tasks = new List<UniTask>();
			foreach (var reopnable in Reopnables)
			{
				if (this != null && reopnable.enabled)
					tasks.Add(reopnable.Load());
			}

			await UniTask.WhenAll(tasks);

			Debug.Log(name + " Load End");
		}

		async UniTask Preopning()
		{
			if (this == null) return;
			Debug.Log(name + " Preopning Start");

			var tasks = new List<UniTask>();
			foreach (var reopnable in Reopnables)
			{
				if (this != null && reopnable.enabled)
					tasks.Add(reopnable.Preopning());
			}

			await UniTask.WhenAll(tasks);

			Debug.Log(name + " Preopning End");
		}

		async UniTask Opening()
		{
			if (this == null) return;
			Debug.Log(name + " Opening Start");

			var tasks = new List<UniTask>();
			foreach (var reopnable in Reopnables)
			{
				if (this != null && reopnable.enabled)
					tasks.Add(reopnable.Opening());
			}

			await UniTask.WhenAll(tasks);

			Debug.Log(name + " Opening End");
		}

		async UniTask OnLoadedUpdate()
		{
			if (this == null) return;
			var tasks = new List<UniTask>();
			foreach (var reopnable in Reopnables)
			{
				if (this != null && reopnable.enabled)
					tasks.Add(reopnable.OnLoadedUpdate());
			}

			await UniTask.WhenAll(tasks);
		}

		async UniTask Closing()
		{
			if (this == null) return;
			Debug.Log(name + " Closing Start");

			var tasks = new List<UniTask>();
			foreach (var reopnable in Reopnables)
			{
				if (this != null && reopnable.enabled)
					tasks.Add(reopnable.Closing());
			}

			await UniTask.WhenAll(tasks);

			Debug.Log(name + " Closing End");
		}
	}
}