using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

namespace ETTView
{
	public static class PhaseTypeEx
    {
		public static async UniTask WaitUntil(this Reopener.PhaseType now, Reopener.PhaseType state)
        {
			await UniTask.WaitUntil(() => now >= state);
		}
    }
	[DisallowMultipleComponent]
	public class Reopener : MonoBehaviour
	{
		public enum PhaseType
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

		public PhaseType Phase { get; private set; } = PhaseType.Loading;

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

		CancellationTokenSource _destroyCts;
		CancellationTokenSource _disableCts;

        public async UniTask Open()
		{
			if (enabled) return;
			enabled = true;

			await UniTask.WaitUntil(() => Phase == PhaseType.Opened);
		}

		public async UniTask Close()
		{
			//ロード中だったら待つ
			await UniTask.WaitWhile(() => Phase == PhaseType.Loading);
			if (!enabled) return;
			enabled = false;

			await UniTask.WaitUntil(() => Phase == PhaseType.Closed);
		}


		protected virtual async void Awake()
		{
            _destroyCts = new CancellationTokenSource();

            await ExecutePhaseAction((reopnable) => reopnable.Loading(), _destroyCts.Token, "Loading");
            Phase = PhaseType.Loaded;
            _onLoaded?.Invoke();
        }

		protected virtual async void OnEnable()
		{
			_disableCts = new CancellationTokenSource();
			var linkedTs = CancellationTokenSource.CreateLinkedTokenSource(_destroyCts.Token, _disableCts.Token);

            try
			{
                //ロード完了まで待つ
                await UniTask.WaitWhile(() => Phase == PhaseType.Loading);

                linkedTs.Token.ThrowIfCancellationRequested();
                Phase = PhaseType.Preopening;
				await ExecutePhaseAction((reopnable) => reopnable.Preopning(), linkedTs.Token, "Preopning");

                linkedTs.Token.ThrowIfCancellationRequested();
                Phase = PhaseType.Opening;
				_onOpen?.Invoke();
				await ExecutePhaseAction((reopnable) => reopnable.Opening(linkedTs.Token), linkedTs.Token, "Opening");

                linkedTs.Token.ThrowIfCancellationRequested();
                Phase = PhaseType.Opened;
				await UniTask.WaitWhile(() => enabled, cancellationToken: _destroyCts.Token);
				
			}
			catch(OperationCanceledException e)
			{
				Debug.Log("Reopner Disable or Destory. " + e.Message);
			}
			catch(Exception e)
			{
				Debug.LogException(e);
			}
			finally
			{
				try
				{
					Phase = PhaseType.Closing;
					_onClose?.Invoke();
					await ExecutePhaseAction((reopnable) => reopnable.Closing(), _destroyCts.Token, "Closing");
					Phase = PhaseType.Closed;
				}
                catch (OperationCanceledException e)
                {
                    Debug.Log("Reopner Destory. " + e.Message);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
		}
		

        async UniTask ExecutePhaseAction(Func<Reopnable, UniTask> action, CancellationToken token, string logMessage)
        {
            token.ThrowIfCancellationRequested();
            Debug.Log(name + " " + logMessage + " Start");

            var tasks = new List<UniTask>();
            foreach (var reopnable in Reopnables)
            {
                if (reopnable.enabled)
                    tasks.Add(action(reopnable));

                token.ThrowIfCancellationRequested();
            }

            await UniTask.WhenAll(tasks);

            token.ThrowIfCancellationRequested();
            Debug.Log(name + " " + logMessage + " End");
        }

        public void OnDisable()
        {
            if(_disableCts != null)
			{
				_disableCts.Cancel();
				_disableCts.Dispose();
				_disableCts = null;
			}
        }

        public void OnDestroy()
		{
			if (_destroyCts != null)
			{
				_destroyCts.Cancel();
				_destroyCts.Dispose();
				_destroyCts = null;
			}
		}
	}
}