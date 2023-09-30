using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETTView.UI;

public class TitleView : UIView
{
    [SerializeField] GameView _gameView;
    public async void OnClickStart()
    {
        await _gameView.Open();
    }
}
