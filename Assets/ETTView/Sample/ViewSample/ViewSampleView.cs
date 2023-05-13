using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETTView;
using ETTView.UI;

public class ViewSampleView : UIView
{
	public void Update()
	{
		if (State == Reopener.StateType.Opened)
		{
			//‰ñ“]‚µ‚Â‚Ã‚¯‚é
			transform.Rotate(new Vector3(0, 0, 1));
		}
	}
}

