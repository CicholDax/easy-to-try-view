using UnityEngine;
using Cysharp.Threading.Tasks;

namespace ETTView.UI
{
    public class Popup : Reopnable
    {
		public static async UniTask<T> Create<T>(UIView view = null) where T : Popup
		{
			var req = await Resources.LoadAsync<T>(typeof(T).Name) as T;
			var ins = Instantiate(req, view.PopupParent);

			return ins;
		}
	}
}