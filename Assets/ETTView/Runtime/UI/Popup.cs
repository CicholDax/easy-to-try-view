using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace ETTView.UI
{
    public class Popup : ReopnablePrefab
	{
        public virtual bool CanBackPopup()
        {
            return true;
        }

        public override UniTask Preopning(CancellationToken token)
		{
			UIViewManager.Instance.Current.RegistPopup(this);
			return base.Preopning(token);
		}
	}
}