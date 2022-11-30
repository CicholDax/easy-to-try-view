using UnityEngine;
using Cysharp.Threading.Tasks;

namespace ETTView.UI
{
    public class Popup : Reopnable
    {
		public static async UniTask<T> CreateFromResources<T>(UIView view = null) where T : Popup
		{
			if (view == null) view = UIViewManager.Instance.Current;
			var req = await Resources.LoadAsync<T>(typeof(T).Name) as T;
			var ins = Instantiate(req, view.PopupParent);

			return ins;
		}

		public static Popup CreateFromPrefab(Popup prefab, UIView view = null)
		{
			if (view == null) view = UIViewManager.Instance.Current;
			var ins = Instantiate(prefab, view.PopupParent);

			return ins;
		}
	}
}