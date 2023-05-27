using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ETTView
{
    public class ExternalConfigApplier : Reopnable
    {
		[SerializeField] UnityEvent _onApply;

		public sealed override UniTask Opening()
		{
			ExternalConfigManager.Instance.Regist(this);
			return base.Opening();
		}

		public sealed override UniTask Closing()
		{
			ExternalConfigManager.Instance.UnRegist(this);
			return base.Closing();
		}

		public virtual UniTask Apply()
        {
			_onApply?.Invoke();
			return UniTask.CompletedTask;
		}
    }
}
