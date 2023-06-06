using UnityEngine;
using Cysharp.Threading.Tasks;

namespace ETTView.UI
{
    public class Popup : ReopnablePrefab
	{
        public virtual bool CanBackPopup()
        {
            return true;
        }

        public override UniTask Preopning()
		{
			UIViewManager.Instance.Current.RegistPopup(this);
			return base.Preopning();
		}
	}
}