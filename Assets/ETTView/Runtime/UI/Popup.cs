using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace ETTView.UI
{
    public class Popup : ReopnablePrefab
	{
		public UIView View => UIViewManager.Instance.Current;

		public virtual bool CanBackPopup()
        {
            return true;
        }

        public override UniTask Opening(CancellationToken token)
		{
			UIViewManager.Instance.Current.RegistPopup(this);
			return base.Preopning(token);
		}
	}
}