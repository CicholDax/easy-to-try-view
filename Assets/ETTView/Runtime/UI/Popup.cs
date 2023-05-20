using UnityEngine;
using Cysharp.Threading.Tasks;

namespace ETTView.UI
{
    public class Popup : ReopnablePrefab
	{
		public void BackView()
		{
			UIViewManager.Instance.Current.BackView();
		}

		public override UniTask Preopning()
		{
			UIViewManager.Instance.Current.RegistPopup(this);
			return base.Preopning();
		}
	}
}