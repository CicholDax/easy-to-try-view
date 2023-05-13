using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

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

		CancellationTokenSource _cts;

		public async UniTask Open()
		{
			if (_cts.Token.IsCancellationRequested) return;
			if (enabled) return;
			enabled = true;

			await UniTask.WaitUntil(() => State == StateType.Opened);
		}

		public async UniTask Close()
		{
			if (_cts.Token.IsCancellationRequested) return;
			//ロード中だったら待つ
			await UniTask.WaitWhile(() => State == StateType.Loading);
			if (!enabled) return;
			enabled = false;

			await UniTask.WaitUntil(() => State == StateType.Closed);
		}


		protected virtual async void Awake()
		{
			_cts = new CancellationTokenSource();

			UniTaskScheduler.UnobservedExceptionWriteLogType = LogType.Warning;

			await Load(_cts.Token);
			State = StateType.Loaded;
		}

		protected virtual async void OnEnable()
		{
			try
			{
				//ロード完了まで待つ
				await UniTask.WaitWhile(() => State == StateType.Loading);

				_onLoaded?.Invoke();

				State = StateType.Preopening;

				await Preopning(_cts.Token);

				State = StateType.Opening;

				_onOpen?.Invoke();

				await Opening(_cts.Token);

				State = StateType.Opened;

				await UniTask.WaitWhile(() => enabled, cancellationToken: _cts.Token);

				State = StateType.Closing;

				_onClose?.Invoke();

				await Closing(_cts.Token);

				State = StateType.Closed;
			}
			catch(OperationCanceledException e)
			{
				Debug.Log("Reopner. " + e.Message);
			}
		}

		async UniTask Load(CancellationToken token)
		{
			token.ThrowIfCancellationRequested();
			Debug.Log(name + " Load Start");

			var tasks = new List<UniTask>();
			foreach (var reopnable in Reopnables)
			{
				if (reopnable.enabled)
					tasks.Add(reopnable.Load());

				token.ThrowIfCancellationRequested();
			}

			await UniTask.WhenAll(tasks);

			token.ThrowIfCancellationRequested();
			Debug.Log(name + " Load End");
		}

		async UniTask Preopning(CancellationToken token)
		{
			token.ThrowIfCancellationRequested();
			Debug.Log(name + " Preopning Start");

			var tasks = new List<UniTask>();
			foreach (var reopnable in Reopnables)
			{
				if (reopnable.enabled)
					tasks.Add(reopnable.Preopning());

				token.ThrowIfCancellationRequested();
			}

			await UniTask.WhenAll(tasks);

			token.ThrowIfCancellationRequested();
			Debug.Log(name + " Preopning End");
		}

		async UniTask Opening(CancellationToken token)
		{
			token.ThrowIfCancellationRequested();
			Debug.Log(name + " Opening Start");

			var tasks = new List<UniTask>();
			foreach (var reopnable in Reopnables)
			{
				if (reopnable.enabled)
					tasks.Add(reopnable.Opening());

				token.ThrowIfCancellationRequested();
			}

			await UniTask.WhenAll(tasks);

			token.ThrowIfCancellationRequested();
			Debug.Log(name + " Opening End");
		}

		async UniTask Closing(CancellationToken token)
		{
			token.ThrowIfCancellationRequested();
			Debug.Log(name + " Closing Start");

			var tasks = new List<UniTask>();
			foreach (var reopnable in Reopnables)
			{
				if (reopnable.enabled)
					tasks.Add(reopnable.Closing());

				token.ThrowIfCancellationRequested();
			}

			await UniTask.WhenAll(tasks);

			token.ThrowIfCancellationRequested();
			Debug.Log(name + " Closing End");
		}

		public void OnDestroy()
		{
			if (_cts != null)
			{
				_cts.Cancel();
				_cts.Dispose();
				_cts = null;
			}
		}
	}
}