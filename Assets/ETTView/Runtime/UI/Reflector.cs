using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ETTView.UI
{
    public class Reflector : Reopnable
    {
		[SerializeField] UnityEvent _onReflect;

		public sealed override UniTask Opening()
		{
			UIViewManager.Instance.Current.RegistReflector(this);	//���f����
			return base.Opening();
		}

		public sealed override UniTask Closing()
		{
			UIViewManager.Instance.Current.UnRegistReflector(this);	//���f����
			return base.Closing();
		}

		public virtual UniTask Reflect()
        {
			_onReflect?.Invoke();
			return UniTask.CompletedTask;
		}
    }
}
