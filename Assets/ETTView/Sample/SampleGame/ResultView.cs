using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETTView.UI;
using Cysharp.Threading.Tasks;

public class ResultView : UIView
{
    public static async UniTask<ResultView> Create(Transform parent)
    {
        return await CreateFromResources<ResultView>(parent);
    }

    public async void OnClickBackToTtile()
    {
        await BackToTargetView<TitleView>();
    }
}
