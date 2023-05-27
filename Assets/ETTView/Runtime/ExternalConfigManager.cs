using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ETTView
{
	internal class ExternalConfigManager : Singleton<ExternalConfigManager>
	{
		Stack<ExternalConfigApplier.IConfigData> _history = new Stack<ExternalConfigApplier.IConfigData>();

		public void Regist(ExternalConfigApplier.IConfigData applier)
		{
			_history.Push(applier);
			applier.Apply();
		}

		public void UnRegist(ExternalConfigApplier.IConfigData applier)
		{
			//���L���ɂȂ��Ă�applier��������O�̂�L���ɂ���
			if (_history.Count <= 0) return;
			if (_history.Peek() == applier)
			{
				_history.Pop();
				var nextReflector = _history.Peek();
				if (nextReflector != null) nextReflector.Apply();
			}
			else
			{
				_history.Pop();
			}
		}
	}
}