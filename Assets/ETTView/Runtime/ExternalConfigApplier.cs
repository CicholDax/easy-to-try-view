using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

		public abstract IConfigData ConfigData { get; }

		public sealed override UniTask Opening(CancellationToken token)
		{
			ExternalConfigManager.Instance.Regist(ConfigData);
			return base.Opening(token);
		}

		public sealed override UniTask Closing()
		{
			ExternalConfigManager.Instance.UnRegist(ConfigData);
			return base.Closing();
		}

		public void OnDestroy()
		{
			ExternalConfigManager.Instance.UnRegist(ConfigData);
		}
	}
}
