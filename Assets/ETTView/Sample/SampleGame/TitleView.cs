using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETTView.UI;
using UnityEngine.SceneManagement;

namespace ETTView.SampleGame
{
    public class TitleView : UIView
    {
        public async void OnClickStart()
        {
            SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
        }
    }
}