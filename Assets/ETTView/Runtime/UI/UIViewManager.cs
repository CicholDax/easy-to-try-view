using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using System;

namespace ETTView.UI
{
	public class UIViewManager : SingletonMonoBehaviour<UIViewManager>, ISingletonMono
	{
		public bool IsDontDestroy { get; } = true;

		Stack<UIView> _history = new Stack<UIView>();

		public UIView Current
		{
			get
			{
                while (_history.Count > 0 && _history.Peek() == null)
                {
                    _history.Pop(); //nullの要素を削除する
                }

                if (_history.Count <= 0) return null;
				return _history.Peek();
			}
		}

        public IEnumerable<UIView> History
        {
            get { return _history; }
        }

        //登録
        //ビューが生成された時に登録する
        public async UniTask Regist(UIView newView)
		{
			var tasks = new List<UniTask>();
			if (!_history.Contains(newView))
			{
				foreach (var view in _history)
				{
					if (view == null)
					{
						Debug.LogWarning("破棄されたUIViewがHistoryに残っています。SingleでSceneLoadする場合は戻れなくなるのでUIViewManager.ClearHistoryしてください。");
						continue;
					}
					//ビューはひとつしかOpenにならないので、他はClose
					tasks.Add(view.Close());
				}

				_history.Push(newView);
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

		//一番最初に挿入
		public async UniTask Interrupt(UIView view)
		{
			await view.Close();

			var list = new List<UIView>(_history.ToArray());
			list.Reverse();

			_history.Clear();

			_history.Push(view);
			foreach(var v in list)
			{
				_history.Push(v);
			}
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

		public async UniTask BackToTargetView(Func<UIView, bool> predicate)
        {
            var list = new List<UIView>(_history.ToArray());
            list.Reverse();

			List<UniTask> tasks = new List<UniTask>();
            _history.Clear();
			bool hit = false;
            foreach (var v in list)
            {
				if (!hit)
				{
					_history.Push(v);
					if (predicate(v))
					{
						tasks.Add(v.Open());
						hit = true;
					}
				}
				else
				{
                    tasks.Add(v.Close());
                }
            }
        }

		//履歴から削除
		public void Remove(UIView view)
		{
			var list = new List<UIView>(_history);
			list.Remove(view);
			list.Reverse();
            _history = new Stack<UIView>(list);
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
		public async UniTask<bool> BackView(Reopnable target, bool isForceBackView = true)
		{
            if (Current.LastPopup == target)
            {
                await Current.TryCloseLastPopup();
                return true;
            }

            if ( Current.CurrentState == target )
			{
				await Current.TryBackState();
				return true;
			}

			if(Current == target)
			{
				return await BackView(false, false, isForceBackView);
			}

			return false;
		}

		/// <summary>
		/// 画面を戻る
		/// </summary>
		/// <param name="isClosePopup">ポップアップを閉じるかどうか</param>
		/// <param name="isBackState">ステートを戻るかどうか</param>
		/// <param name="isForceBackView">UIViewの定義に関わらず強制的にViewを戻るかどうか</param>
		/// <returns>Popupが閉じる、Stateが戻る、UIViewが戻るしたらtrue</returns>
		public async UniTask<bool> BackView(bool isClosePopup = true, bool isBackState = true, bool isForceBackView = false)
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
					tasks.Add(view.Close(true));
					Remove(view);

					//今のをCloseしたのでCurrentは次のやつになってる
					Current.SetRewind(true);
					var openAndRewindTask = Current.Open().ContinueWith(() => Current.SetRewind(false));
					tasks.Add(openAndRewindTask);

					await UniTask.WhenAll(tasks);

					return true;
				}
			}
			else
			{
				Debug.LogWarning("RootViewなので閉じられません");
			}
			return false;
		}

		public  void ClearHistory()
		{
			_history.Clear();
		}

		public async void Update()
		{
			if (Current != null && Current.Phase == Reopener.PhaseType.Opened && Current.IsBackInput())
			{
				await BackView();
			}
        }
	}
}