using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETTView;
using ETTView.UI;

public class ViewSampleView : UIView
{
	public void Update()
	{
		if (Phase == Reopener.PhaseType.Opened)
		{
			//回転しつづける
			//transform.Rotate(new Vector3(0, 0, 1));
		}
	}
}

