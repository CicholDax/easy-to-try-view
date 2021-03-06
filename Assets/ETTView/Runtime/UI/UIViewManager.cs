using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

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
				if (_history.Count <= 0) return null;
				return _history.Peek();
			}
		}

		//登録
		//ビューが生成された時に登録する
		public async UniTask Regist(UIView newView)
		{
			var tasks = new List<UniTask>();
			foreach (var view in _history)
			{
				//ビューはひとつしかOpenにならないので、他はClose
				tasks.Add(view.Close());
			}

			_history.Push(newView);

			await UniTask.WhenAll(tasks);
		}

		//履歴から削除
		public void Remove(UIView view)
		{
			var list = new List<UIView>(_history);
			list.Remove(view);
			_history = new Stack<UIView>(list);
		}

		//新しいビューを生成する
		public async UniTask<T> Create<T>() where T : UIView
		{
			//var tasks = new List<UniTask>();

			//現在のビューを閉じる	メモ：生成したあとのRegistでどうせCloseするから消してみる。UIViewに初期値入れるため
			//tasks.Add(Current.Close());

			var parent = Current != null ? Current.transform.parent : null;
			var req = await Resources.LoadAsync<T>(typeof(T).Name) as T;
			var ins = Instantiate(req, parent);

			//await UniTask.WhenAll(tasks);

			return ins;
		}

		public async UniTask WaitUntil(Reopener.StateType state)
		{
			await UniTask.WaitUntil(() => Current.State >= state);
		}		

		public async UniTask BackView(bool isClosePopup = true, bool isForceBackView = false)
		{
			await Current.BackView(
				_history.Count <= 1,
				isClosePopup,
				isForceBackView,
				async () =>
				{
					_history.Pop();
					Current.SetRewind(true);
					await Current.Open();
					Current.SetRewind(false);
				}
			);
		}

		public  void ClearHistory()
		{
			_history.Clear();
		}

		public async void Update()
		{
			if (Current != null && Current.State == Reopener.StateType.Opened && Input.GetKeyDown(KeyCode.Escape))
			{
				await BackView();
			}
		}
	}
}