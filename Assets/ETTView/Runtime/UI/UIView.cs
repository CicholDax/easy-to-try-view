using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace ETTView.UI
{
	public class UIView : Reopnable
	{
		//シーンに最初からRootに置かれてるビューかどうか
		[SerializeField] bool _isSceneTopView;

		//BackSceneで戻る/戻られる場合にトランジションを変更したい場合に指定する
		[SerializeField] List<Reopnable> _forwardTransitions;
		[SerializeField] List<Reopnable> _rewindTransitions;

		//このビューが有効な時に開いたポップアップのリスト
		[SerializeField]List<Popup> _openedPopupList = new List<Popup>();

		//状態遷移履歴
		Stack<UIViewState> _stateHistory = new Stack<UIViewState>();

		//BackSceneで戻ってきた場合にtrue
		public bool IsRewind
		{
			get;set;
		}

		//最後に開いたポップアップ
		internal Popup LastPopup
		{
			get
			{
				Popup pop = null;
				for(var i = _openedPopupList.Count-1; i >= 0; i-- )		
				{
					pop = _openedPopupList[i];

					//Nullだったりまだ開いてなかったら無視
					if (pop == null || pop.gameObject == null || pop.Phase != Reopener.PhaseType.Opened)
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

		internal UIViewState CurrentState
		{
			get
			{
				return _stateHistory.Peek();
			}
		}

		public async UniTask RegistState(UIViewState state)
		{
			List<UniTask> tasks = new List<UniTask>();
			foreach( var historyState in _stateHistory )
			{
				if(historyState != state)
					tasks.Add(historyState.Close());
			}
			if (_stateHistory.Count <= 0 || _stateHistory.Peek() != state)
				_stateHistory.Push(state);

			await UniTask.WhenAll(tasks);
		}

		public void RegistPopup(Popup popup)
		{
			if (_openedPopupList.Contains(popup))
			{
				_openedPopupList.Remove(popup);
			}

			_openedPopupList.Add(popup);
		}

		public override UniTask Close(bool destroy = false)
		{
			if(destroy)
			{
				UIViewManager.Instance.Remove(this);
			}

			return base.Close(destroy);
		}

		public void SetRewind(bool flag)
		{
			IsRewind = flag;
			foreach (var transition in _rewindTransitions)
			{
				transition.enabled = flag;
			}
			foreach (var transition in _forwardTransitions)
			{
				transition.enabled = !flag;
			}
		}

		public async void Awake()
		{
			SetRewind(false);

			//マネージャに登録
			await UIViewManager.Instance.Regist(this);
		}

		//Viewを戻ろうとしたタイミングで呼ばれる。戻って良ければtrueを返す
		public virtual bool CanBackView()
		{
			return true;
		}

		public async UniTask<bool> TryCloseLastPopup()
		{
			var lastPopup = LastPopup;
			if (lastPopup != null)
			{
				//ポップアップが開いてたら閉じる
				await lastPopup.Close();
				return true;
			}

			return false;
		}
		
		public async UniTask<bool> TryBackState()
		{
			if (_stateHistory.Count > 1)
			{
				//ステートを戻る
				var closeState = _stateHistory.Pop();
				var openState = _stateHistory.Pop();

				if (openState.AwaitCloseState)
					await closeState.Close();
				else
					closeState.Close().Forget();

				await openState.Open();

				return true;
			}

			return false;
		}

		public async void BackViewNowait()
		{
			await UIViewManager.Instance.BackView();
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
