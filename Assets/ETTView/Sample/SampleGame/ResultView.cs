using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETTView.UI;

public class ResultView : UIView
{
    public async void OnClickBackToTtile()
    {
        await BackToTargetView<TitleView>();
    }
}
