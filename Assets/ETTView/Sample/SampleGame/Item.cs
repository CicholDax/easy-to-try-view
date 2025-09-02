using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace ETTView.SampleGame
{
    [RequireComponent(typeof(Collider))]
    public class Item : ReopenablePrefab
    {
        public static UniTask<Item> Create(Transform parent)
        {
            return CreateFromResources<Item>(parent);
        }
    }
}