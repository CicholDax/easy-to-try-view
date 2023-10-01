using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(Collider))]
public class Item : ReopnablePrefab
{
    public static UniTask<Item> Create(Transform parent)
    {
        return CreateFromResources<Item>(parent);
    }
}
