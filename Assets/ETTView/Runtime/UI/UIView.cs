using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using JetBrains.Annotations;

namespace ETTView.UI
{
	public class UIView : BackableReopnablePrefab
	{
		public static UIView Current => UIViewManager.Instance.Current;
		public static IEnumerable<UIView> History => UIViewManager.Instance.History;

		//クローズ時にDestroyするかどうかのフラグ
		protected override bool IsDestroyWhenClosed => _isSceneTopView;

		//シーンに最初からRootに置かれてるビューかどうか
		[SerializeField] bool _isSceneTopView;

		//BackSceneで戻る/戻られる場合にトランジションを変更したい場合に指定する
		[SerializeField] List<Reopnable> _forwardTransitions;
		[SerializeField] List<Reopnable> _rewindTransitions;

		//このビューが有効な時に開いたポップアップのリスト
		[SerializeField] List<UIViewPopup> _openedPopupList = new List<UIViewPopup>();

		//状態遷移履歴
		Stack<UIViewState> _stateHistory = new Stack<UIViewState>();

		public IEnumerable<UIViewPopup> OpenedPopups
		{
			get { return _openedPopupList; }
		}

		public IEnumerable<UIViewState> StateHistory
		{
			get { return _stateHistory; }
		}

		//BackSceneで戻ってきた場合にtrue
		public bool IsRewind
		{
			get; set;
		}

		public bool IsOtherViewCloset
		{
			get
			{
				foreach (var view in UIViewManager.Instance.History)
				{
					if (view != this)
					{
						//自分以外が開いてたらFalse
						if (view.IsOpen) return false;
					}
				}
				return true;
			}
		}

		//最後に開いたポップアップ
		internal UIViewPopup LastPopup
		{
			get
			{
				UIViewPopup pop = null;
				for (var i = _openedPopupList.Count - 1; i >= 0; i--)
				{
					pop = _openedPopupList[i];

					//Nullだったりまだ開いてなかったら無視
					if (pop == null || pop.gameObject == null || pop.Phase != Reopener.PhaseType.Opened || !pop.gameObject.activeInHierarchy)
					{
						pop = null;
					}

					if (pop != null) break;
				}


				//不要な要素を削除
				_openedPopupList.RemoveAll((data) => data == null || data.gameObject == null);

				return pop;
			}
		}

		public bool ExistPopup()
		{
			return LastPopup != null;
		}

		internal UIViewState CurrentState
		{
			get
			{
				return _stateHistory.Count > 0 ? _stateHistory.Peek() : null;
			}
		}

		public async UniTask RegistState(UIViewState state)
		{
			List<UniTask> tasks = new List<UniTask>();
			foreach (var historyState in _stateHistory)
			{
				if (historyState != state)
					tasks.Add(historyState.Close());
			}
			if (_stateHistory.Count <= 0 || _stateHistory.Peek() != state)
				_stateHistory.Push(state);

			await UniTask.WhenAll(tasks);
		}

		public void RegistPopup(UIViewPopup popup)
		{
			if (_openedPopupList.Contains(popup))
			{
				_openedPopupList.Remove(popup);
			}

			_openedPopupList.Add(popup);
		}

		public override UniTask Close(bool destroy = false)
		{
			if (destroy)
			{
				UIViewManager.Instance.Remove(this);
			}

			return base.Close(destroy);
		}

		internal void SetRewind(bool flag)
		{
			IsRewind = flag;
			foreach (var transition in _rewindTransitions)
			{
				if (transition != null)
					transition.enabled = flag;
			}
			foreach (var transition in _forwardTransitions)
			{
				if (transition != null)
					transition.enabled = !flag;
			}
		}

		public override async UniTask Preopning(CancellationToken token)
		{
			//マネージャに登録
			SetRewind(false);
			await UIViewManager.Instance.Regist(this);
		}

		//Viewを戻ろうとしたタイミングで呼ばれる。戻って良ければtrueを返す
		public virtual bool CanBackView()
		{
			return true;
		}

		public virtual bool IsBackInput()
		{
			//デフォルトだとエスケープで戻る
			return Input.GetKeyDown(KeyCode.Escape);
		}

		/// <summary>
		/// 最後に開いたポップアップを閉じる
		/// </summary>
		/// <returns></returns>
		internal async UniTask<bool> TryCloseLastPopup()
		{
			var lastPopup = LastPopup;
			if (lastPopup != null)
			{
				if (lastPopup.CanBackPopup())
				{
					//ポップアップが開いてたら閉じる
					await lastPopup.CloseAndDestroyIfNeeded();
				}

				return true;
			}

			return false;
		}

		/// <summary>
		/// 最後のステートから戻る
		/// </summary>
		/// <returns></returns>
		internal async UniTask<bool> TryBackState()
		{
			//nullの要素を除外
			_stateHistory = new Stack<UIViewState>(_stateHistory.Where(state => state != null && state.gameObject != null).Reverse());

			if (_stateHistory.Count > 1)
			{
				//ステートを戻る
				var closeState = _stateHistory.Pop();
				var openState = _stateHistory.Pop();

				if (openState.AwaitCloseState)
					await closeState.CloseAndDestroyIfNeeded();
				else
					closeState.CloseAndDestroyIfNeeded().Forget();

				await openState.Open();

				return true;
			}

			return false;
		}

		/// <summary>
		/// 指定のビューまで戻る（UnityEventに設定する用）
		/// </summary>
		/// <param name="view"></param>
		public void BackToTargetViewForget(UIView view)
		{
			BackToTargetView(view).Forget();
		}

		/// <summary>
		/// 指定のビューまで戻る（型指定）
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public UniTask BackToTargetView<T>() where T : UIView
		{
			return UIViewManager.Instance.BackToTargetView<T>();
		}

		/// <summary>
		/// 指定のビューまで戻る（インスタンス指定）
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
        public UniTask BackToTargetView(UIView view)
        {
            return UIViewManager.Instance.BackToTargetView(view);
        }

        public void OnDestroy()
		{
			if(_isSceneTopView)
			{
				//読み込み済みのシーンからこのViewが含まれるシーンを探す
				for (var i = 0; i < SceneManager.sceneCount; i++)
				{
					var scene = SceneManager.GetSceneAt(i);

					if( gameObject.scene == scene )
					{
						//シーンごと破棄する
						SceneManager.UnloadSceneAsync(scene);

						Debug.Log(name + "によって" + gameObject.scene.name + "が破棄されました。");
					}
				}
			}
		}
	}
}
