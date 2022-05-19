using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using DG.Tweening;
using UniDax;

namespace UniDax.UI
{
	public class UIView : Reopnable
	{
		//シーンに最初から置かれてるビューかどうか
		[SerializeField] bool _isSceneTopView;

		//ポップアップの親
		[SerializeField] Transform _popupParent;

		//BackSceneで戻る/戻られる場合にトランジションを変更したい場合に指定する
		[SerializeField] List<Reopnable> _forwardTransitions;
		[SerializeField] List<Reopnable> _rewindTransitions;

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

		protected Popup LastPopup
		{
			get
			{
				var ret = PopupParent.GetComponentsInChildren<Popup>().LastOrDefault();
				return ret;
			}
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

		public virtual async UniTask BackView( bool isRootView, bool isClosePopup, bool isForceBackView, Func<UniTask> onNextView = null )
		{
			var lastPopup = LastPopup;
			if (lastPopup != null && isClosePopup)
			{
				//ポップアップが開いてたら閉じる
				await lastPopup.Close(true);
			}
			else if (OnBackView() || isForceBackView)
			{
				if (!isRootView)
				{
					List<UniTask> tasks = new List<UniTask>();

					//View自身が閉じる
					SetRewind(true);
					tasks.Add(Close(true));
					if (onNextView != null) tasks.Add(onNextView());

					await UniTask.WhenAll(tasks);
				}
				else
				{
					Debug.LogWarning("RootViewなので閉じられません");
				}
			}
		}

		public async void BackView()
		{
			if (State == Reopener.StateType.Opened)
			{
				await UIViewManager.Instance.BackView();
			}
		}

		public void OnDestroy()
		{
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
							SceneManager.UnloadSceneAsync(scene);
							return;
						}
					}
				}
			}
		}
	}
}
