using Cysharp.Threading.Tasks;
using ETTView.UI;
using ETTView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�v���n�u�Ȃ����O
public class PrefabNotFoundException : System.Exception
{
	public PrefabNotFoundException(string prefabName)
		: base($"ReopnablePrefab: �v���n�u��������܂���ł����B{prefabName}")
	{
	}
}

public class ReopnablePrefab : Reopnable
{
	bool _fromPrefab = false;

	protected virtual bool IsDestroyWhenClosed => _fromPrefab;

	//Resource�����Ɍ^���Ɠ�����Prefab�����݂���O��(����Addressable�ɒu��������)
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
		//�v���n�u���琶������������������
		return base.Close(IsDestroyWhenClosed);
	}
}
