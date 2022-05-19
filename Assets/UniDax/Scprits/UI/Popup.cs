using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using DG.Tweening;
using UniDax;

namespace UniDax.UI
{
    public class Popup : Reopnable
    {
		public static async UniTask<T> Create<T>(UIView view = null) where T : Popup
		{
			var req = await Resources.LoadAsync<T>(typeof(T).Name) as T;
			var ins = Instantiate(req, view.PopupParent);

			return ins;
		}
	}
}