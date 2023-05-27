using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ETTView
{
	internal class ExternalConfigManager : SingletonMonoBehaviour<ExternalConfigManager> , ISingletonMono
	{
		public bool IsDontDestroy => true;

		Dictionary<Type, List<ExternalConfigApplier.IConfigData>> _historys = new Dictionary<Type, List<ExternalConfigApplier.IConfigData>>();

		[ContextMenu("Print History")]
		public void PrintHistory()
		{
			foreach (var entry in _historys)
			{
				var key = entry.Key;
				var stack = entry.Value;

				Debug.Log($"Key: {key}");

				foreach (var configData in stack)
				{
					Debug.Log($"ConfigData: {configData}");
				}
			}
		}

		public void Regist(ExternalConfigApplier.IConfigData applier)
		{
			if (!_historys.ContainsKey(applier.GetType()))
			{
				_historys.Add(applier.GetType(), new List<ExternalConfigApplier.IConfigData>());
			}
			
			_historys[applier.GetType()].Add(applier);
			applier.Apply();
		}

		public void UnRegist(ExternalConfigApplier.IConfigData applier)
		{
			if (!_historys.ContainsKey(applier.GetType())) return;

			//çÌèú
			_historys[applier.GetType()].RemoveAll(x => x == applier);

			//ç≈å„Ç…ìoò^ÇµÇΩÇÃÇîΩâf
			var nextReflector = _historys[applier.GetType()].LastOrDefault();
			if (nextReflector != null) nextReflector.Apply();
		}
	}
}