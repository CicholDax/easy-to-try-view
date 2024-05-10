using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETTView;
using Cysharp.Threading.Tasks;

namespace ETTView.SampleGame
{
    public class PlayerNode : ReopnablePrefab
    {
        float _deadTime = float.MaxValue;

        public static async UniTask<PlayerNode> Create(Transform parent, Vector3 pos, float lifeTime)
        {
            var ret = await CreateFromResources<PlayerNode>(parent);
            ret.transform.position = pos;
            ret._deadTime = Time.time + lifeTime;

            return ret;
        }

        public async void Update()
        {
            if (_deadTime <= Time.time)
            {
                _deadTime = float.MaxValue;
                await Close(true);
            }
        }
    }
}