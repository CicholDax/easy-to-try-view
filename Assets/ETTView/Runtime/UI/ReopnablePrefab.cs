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

public class ReopnablePrefab : Reopnable
{
	bool _fromPrefab = false;

	protected virtual bool IsDestroyWhenClosed => _fromPrefab;

	//Resource直下に型名と同名のPrefabが存在する前提(TODO:Addressableに置き換える)
	protected static async UniTask<T> CreateFromResources<T>(Transform parent, string path = null) where T : ReopnablePrefab
	{
		var req = await Resources.LoadAsync<T>( path == null ? typeof(T).Name : path) as T;
		if (req == null) throw new PrefabNotFoundException(typeof(T).Name);
		var ins = Instantiate(req, parent);
		ins._fromPrefab = true;

		return ins;
	}

    protected static ReopnablePrefab CreateFromPrefab(ReopnablePrefab prefab, Transform parent)
	{
		var ins = Instantiate(prefab, parent);
		ins._fromPrefab = true;

		return ins;
	}

	public UniTask CloseAndDestroyIfNeeded()
	{
		//プレハブから生成したやつだったら消す
		return base.Close(IsDestroyWhenClosed);
	}
}
