using Cysharp.Threading.Tasks;
using ETTView.UI;
using ETTView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレハブないよ例外
public class PrefabNotFoundException : System.Exception
{
	public PrefabNotFoundException(string prefabName)
		: base($"ReopnablePrefab: プレハブが見つかりませんでした。{prefabName}")
	{
	}
}

//Resource直下に型名と同名のPrefabが存在する前提
public class ReopnablePrefab : Reopnable
{
	bool _fromPrefab = false;
	protected virtual bool IsDestroyWhenClosed => _fromPrefab;

	public static async UniTask<T> CreateFromResources<T>(Transform parent) where T : ReopnablePrefab
	{
		var req = await Resources.LoadAsync<T>(typeof(T).Name) as T;
		if (req == null) throw new PrefabNotFoundException(typeof(T).Name);
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
		//プレハブから生成したやつだったら消す
		return base.Close(IsDestroyWhenClosed);
	}
}
