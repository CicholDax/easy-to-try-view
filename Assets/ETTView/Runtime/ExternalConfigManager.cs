using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ETTView
{
	internal class ExternalConfigManager : Singleton<ExternalConfigManager>
	{
		Stack<ExternalConfigApplier> _history = new Stack<ExternalConfigApplier>();

		public void Regist(ExternalConfigApplier applier)
		{
			_history.Push(applier);
			applier.Apply();
		}

		public void UnRegist(ExternalConfigApplier applier)
		{
			_history = new Stack<ExternalConfigApplier>(_history.Where(state => state != null && state.gameObject != null).Reverse());

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