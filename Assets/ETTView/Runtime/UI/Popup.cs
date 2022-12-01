using UnityEngine;
using Cysharp.Threading.Tasks;

namespace ETTView.UI
{
    public class Popup : Reopnable
    {
		bool _fromPrefab = false;
		
		public static async UniTask<T> CreateFromResources<T>(UIView view = null) where T : Popup
		{
			if (view == null) view = UIViewManager.Instance.Current;
			var req = await Resources.LoadAsync<T>(typeof(T).Name) as T;
			var ins = Instantiate(req, view.PopupParent);
			ins._fromPrefab = true;

			return ins;
		}

		public static Popup CreateFromPrefab(Popup prefab, UIView view = null)
		{
			if (view == null) view = UIViewManager.Instance.Current;
			var ins = Instantiate(prefab, view.PopupParent);
			ins._fromPrefab = true;

			return ins;
		}

		public override UniTask Preopning()
		{
			UIViewManager.Instance.Current.RegistPopup(this);
			return base.Preopning();
		}

		public UniTask ClosePopup()
		{
			//ƒvƒŒƒnƒu‚©‚ç¶¬‚µ‚½‚â‚Â‚¾‚Á‚½‚çÁ‚·
			return base.Close(_fromPrefab);
		}
	}
}