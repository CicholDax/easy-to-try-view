using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ETTView.UI;
using UnityEngine.SceneManagement;

namespace ETTView.SampleGame
{
    public class TitleView : UIView
    {
        public override async UniTask Preopning(CancellationToken token)
        {
            await base.Preopning(token);
            await UniTask.WaitUntil(() => IsOtherViewClosed);
        }

        public override async UniTask Opening(CancellationToken token)
        {
            
        }

        public async void OnClickStart()
        {
            SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
        }
    }
}