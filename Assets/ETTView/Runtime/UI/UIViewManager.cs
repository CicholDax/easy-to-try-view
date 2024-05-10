using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.SceneManagement;

namespace ETTView.UI
{
	internal class UIViewManager : SingletonMonoBehaviour<UIViewManager>, ISingletonMono
	{
		public bool IsDontDestroy { get; } = true;

		List<UIView> _history = new List<UIView>();

		//Boot以外から立ち上がったフラグ
		static bool _editorUnitExecute = false;

		public UIView Current
		{
			get
			{
				_history.RemoveAll(x => x == null);
                if (_history.Count <= 0) return null;
                return _history.LastOrDefault();
			}
		}

		UIView Next
		{
			get
			{
				if (_history.Count < 2) return null; // 少なくとも2つの要素が必要
				return _history[^2]; // 最後から二番目の要素を返す
			}
		}

        public IEnumerable<UIView> History
        {
            get { return _history; }
        }

        public void Remove(UIView view)
        {
	        _history.Remove(view);
        }


		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static async void InitializeBeforeSceneLoad()
		{
			//index0のシーンが読み込まれてるかどうか
			for (var i = 0; i < SceneManager.sceneCount; i++)
			{
				var scene = SceneManager.GetSceneAt(i);
				if (scene.buildIndex == 0)
				{
					//読み込まれてたら返る
					return;
				}
			}

			//index0以外の起動だったらフラグ立てる
			_editorUnitExecute = true;

			//index0を読み込む
			await SceneManager.LoadSceneAsync(0, LoadSceneMode.Additive);

			//View割り込み登録（index0シーンに存在するEnableなView→起動したViewの順番に遷移したことにする）
			for (var i = 0; i < SceneManager.sceneCount; i++)
			{
				var scene = SceneManager.GetSceneAt(i);
				if (scene.buildIndex == 0)
				{
					foreach (var go in scene.GetRootGameObjects())
					{
						var views = go.GetComponentsInChildren<UIView>();
						foreach(var view in views)
						{
							if (!view.IsOpen) return;
							await UniTask.WaitUntil(() => view.Phase == Reopener.PhaseType.Opened);	//開ききるのを待つ

							await Instance.Interrupt(view);
						}
					}
				}
			}
		}

		/// <summary>
		/// ビューの登録
		/// Preopeningで呼び出してる
		/// </summary>
		/// <param name="newView"></param>
		public async UniTask Regist(UIView newView)
		{
			var tasks = new List<UniTask>();
			if (!_history.Contains(newView))
			{
				foreach (var view in _history)
				{
					if (view == null)
					{
						Debug.LogWarning("破棄されたUIViewがHistoryに残っています。");
						continue;
					}
					//ビューはひとつしかOpenにならないので、他はClose
					tasks.Add(view.Close());
				}
				
				_history.Add(newView);
			}
			else
			{
                foreach (var view in _history)
                {
					if (view != newView)
						tasks.Add(view.Close());
                }
            }

			await UniTask.WhenAll(tasks);
		}

		/// <summary>
		/// 先頭に割り込んで登録
		/// </summary>
		/// <param name="view"></param>
		async UniTask Interrupt(UIView view)
		{
			await view.Close();
			
			//先頭に入れ替える
			_history.Remove(view);
			_history.Insert(0, view);
			await _history.Last().Open();
		}

		public async UniTask WaitUntil(Reopener.PhaseType state)
		{
			await UniTask.WaitUntil(() => Current.Phase >= state);
		}	
		
		/// <summary>
		/// ターゲットを指定して画面を戻る
		/// 戻る対象が違ったら何もしない
		/// </summary>
		/// <param name="target"></param>
		/// <param name="isForceBackView"></param>
		/// <returns></returns>
		public async UniTask<bool> Back(Reopnable target, bool isForceBackView = true)
		{
			//最後に開いたポップアップを指定してたら
            if (Current.LastPopup == target)
            {
				//ポップアップを閉じる
                await Current.TryCloseLastPopup();
                return true;
            }

			//現在のステートを指定してたら
            if ( Current.CurrentState == target )
			{
				//ステートを戻す
				await Current.TryBackState();
				return true;
			}

			//現在のViewを指定してたら
			if(Current == target)
			{
				//ビューを戻す
				return await Back(false, false, isForceBackView);
			}

			return false;
		}

		/// <summary>
		/// 画面を戻る
		/// </summary>
		/// <param name="isClosePopup">ポップアップを閉じるかどうか</param>
		/// <param name="isBackState">ステートを戻るかどうか</param>
		/// <param name="isForceBackView">UIView.CanBackViewに関わらず強制的にViewを戻るかどうか</param>
		/// <returns>Popupが閉じる、Stateが戻る、UIViewが戻るしたらtrue</returns>
		public async UniTask<bool> Back(bool isClosePopup = true, bool isBackState = true, bool isForceBackView = false)
		{
			//Popupを閉じる
			if (isClosePopup && await Current.TryCloseLastPopup()) return true;

			//UIViewStateを戻る
			if (isBackState && await Current.TryBackState()) return true;

			//UIViewを戻る
			if(_history.Count(x => x != null) > 1)
			{
				if (Current.CanBackView() || isForceBackView)
				{
					//今のを閉じるのと、次のを開けるのを平行してやる
					List<UniTask> tasks = new List<UniTask>();

					//今のを閉じる
					Current.SetRewind(true);
					var view = Current;
					tasks.Add(view.CloseAndDestroyIfNeeded());

					//次を開く
					var nextView = Next;
					nextView.SetRewind(true);
					var openAndRewindTask = nextView.Open().ContinueWith(() => nextView.SetRewind(false));
					tasks.Add(openAndRewindTask);

					await UniTask.WhenAll(tasks);
					
					//閉じるのを待ってからリストから消す
					await UniTask.WaitUntil(() => !view.IsOpen);
					_history.Remove(view);

					return true;
				}
			}
			else
			{
				Debug.LogWarning("RootViewなので閉じられません");
			}
			return false;
		}

		//指定のビューまで戻る
		public UniTask BackToTargetView(UIView view)
		{
			return BackToTargetView(x => x == view);
		}

		public UniTask BackToTargetView<T>() where T : UIView
		{
			return BackToTargetView(x => x.GetType() == typeof(T));
		}

		async UniTask BackToTargetView(Func<UIView, bool> predicate)
		{
			var list = new List<UIView>(_history);
			list.Reverse();

			List<UniTask> tasks = new List<UniTask>();
			_history.Clear();
			bool hit = false;
			foreach (var v in list)
			{
				if (!hit)
				{
					_history.Add(v);
					if (predicate(v))
					{
						tasks.Add(v.Open());
						hit = true;
					}
				}
				else
				{
					tasks.Add(v.Close(true));
				}
			}
		}


		public void ClearHistory()
		{
			_history.Clear();
		}

		public async void Update()
		{
			if (Current != null && Current.Phase == Reopener.PhaseType.Opened && Current.IsBackInput())
			{
				await Back();
			}
        }
	}
}