using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ETTView
{
    public abstract class ExternalConfigApplier : Reopnable
	{
		public interface IConfigData
		{
			void Apply();
		}

		public sealed override UniTask Opening()
		{
			ExternalConfigManager.Instance.Regist(GetConfigData());
			return base.Opening();
		}

		public sealed override UniTask Closing()
		{
			ExternalConfigManager.Instance.UnRegist(GetConfigData());
			return base.Closing();
		}

		public abstract IConfigData Apply();
		public abstract IConfigData GetConfigData();
    }
}
