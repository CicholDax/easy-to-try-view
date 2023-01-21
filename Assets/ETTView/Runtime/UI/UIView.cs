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

		//ポップアップの親
		[SerializeField] Transform _popupParent;

		//BackSceneで戻る/戻られる場合にトランジションを変更したい場合に指定する
		[SerializeField] List<Reopnable> _forwardTransitions;
		[SerializeField] List<Reopnable> _rewindTransitions;

		//このビューが有効な時に開いたポップアップのリスト
		[SerializeField]List<Popup> _openedPopupList = new List<Popup>();

		//BackSceneで戻ってきた場合にtrue
		public bool IsRewind
		{
			get;set;
		}
		
		public Transform PopupParent
		{
			get
			{
				return _popupParent != null ? _popupParent : transform;
			}
		}

		//最後に開いたポップアップ
		protected Popup LastPopup
		{
			get
			{
				Popup pop = null;
				for(var i = _openedPopupList.Count-1; i >= 0; i-- )		
				{
					pop = _openedPopupList[i];

					//Nullだったりまだ開いてなかったら無視
					if (pop.gameObject == null || pop.State != Reopener.StateType.Opened)
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

		public void RegistPopup(Popup popup)
		{
			if (_openedPopupList.Contains(popup))
			{
				_openedPopupList.Remove(popup);
			}

			_openedPopupList.Add(popup);
		}

		/*
		public override async UniTask Closing()
		{
			//トップビューのクローズ後処理
			if(_isSceneTopView)
			{
				//読み込み済みのシーンからこのViewが含まれるシーンを探す
				for (var i = 0; i < SceneManager.sceneCount; i++)
				{
					var scene = SceneManager.GetSceneAt(i);

					foreach (var go in scene.GetRootGameObjects())
					{
						if (go == transform.root.gameObject)
						{
							//シーンごと破棄する
							await SceneManager.UnloadSceneAsync(scene);
						}
					}
				}
				
			}
		}
		*/

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
		public virtual bool OnBackView()
		{
			return true;
		}

		internal virtual async UniTask BackView( bool isRootView, bool isClosePopup, bool isForceBackView, Func<UniTask> onNextView = null )
		{
			var lastPopup = LastPopup;
			if (lastPopup != null && isClosePopup)
			{
				//ポップアップが開いてたら閉じる
				await lastPopup.ClosePopup();
			}
			else if (OnBackView() || isForceBackView)
			{
				if (!isRootView)
				{
					List<UniTask> tasks = new List<UniTask>();

					//View自身が閉じる
					SetRewind(true);
					tasks.Add(base.Close(true));
					if (onNextView != null) tasks.Add(onNextView());

					await UniTask.WhenAll(tasks);
				}
				else
				{
					Debug.LogWarning("RootViewなので閉じられません");
				}
			}
		}

		public async void BackView()	//OnBackView　院ベント用なんだったらOnBackViewにしたいな
		{
			if (State == Reopener.StateType.Opened)
			{
				await UIViewManager.Instance.BackView();
			}
		}

		//このビューが存在するシーンを読み込み
		/*
		public Scene? GetScene()
		{
			if (!_isSceneTopView) return null;

			//読み込み済みのシーンからこのViewが含まれるシーンを探す
			for (var i = 0; i < SceneManager.sceneCount; i++)
			{
				var scene = SceneManager.GetSceneAt(i);

				foreach (var go in scene.GetRootGameObjects())
				{
					if (go == transform.root.gameObject)
					{
						return scene;
					}
				}
			}

			return null;
		}
		*/

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
					}
				}
			}
		}
	}
}
