using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ETTView
{
	internal class ExternalConfigManager : Singleton<ExternalConfigManager>
	{
		Dictionary<Type, Stack<ExternalConfigApplier.IConfigData>> _historys = new Dictionary<Type, Stack<ExternalConfigApplier.IConfigData>>();

		public void Regist(ExternalConfigApplier.IConfigData applier)
		{
			if (!_historys.ContainsKey(applier.GetType()))
			{
				_historys.Add(applier.GetType(), new Stack<ExternalConfigApplier.IConfigData>());
			}
			
			_historys[applier.GetType()].Push(applier);
			applier.Apply();
		}

		public void UnRegist(ExternalConfigApplier.IConfigData applier)
		{
			if (!_historys.ContainsKey(applier.GetType())) return;
			if (_historys[applier.GetType()].Count <= 0) return;

			//¡—LŒø‚É‚È‚Á‚Ä‚éapplier‚¾‚Á‚½‚ç‘O‚Ì‚ð—LŒø‚É‚·‚é
			if (_historys[applier.GetType()].Peek() == applier)
			{
				_historys[applier.GetType()].Pop();
				var nextReflector = _historys[applier.GetType()].Peek();
				if (nextReflector != null) nextReflector.Apply();
			}
			else
			{
				_historys[applier.GetType()].Pop();
			}
		}
	}
}