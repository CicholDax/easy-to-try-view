using Cysharp.Threading.Tasks;
using ETTView.UI;
using ETTView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReopnablePrefab : Reopnable
{
	bool _fromPrefab = false;

	public static async UniTask<T> CreateFromResources<T>(Transform parent) where T : ReopnablePrefab
	{
		var req = await Resources.LoadAsync<T>(typeof(T).Name) as T;
		var ins = Instantiate(req, parent);
		ins._fromPrefab = true;

		return ins;
	}

	public static ReopnablePrefab CreateFromPrefab(ReopnablePrefab prefab, Transform parent)
	{
		var ins = Instantiate(prefab, parent);
		ins._fromPrefab = true;

		return ins;
	}

	public UniTask Close()
	{
		//ƒvƒŒƒnƒu‚©‚ç¶¬‚µ‚½‚â‚Â‚¾‚Á‚½‚çÁ‚·
		return base.Close(_fromPrefab);
	}
}
