using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorBundle : Graphic
{
	[SerializeField]List<Graphic> _targets;

	public override Color color
	{
		get => base.color;
		set
		{
			foreach(var target in _targets)
			{
				target.color = value;
			}
		}
	}

	public override void CrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha)
	{
		foreach (var target in _targets)
		{
			target.CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha);
		}
	}

	public override void CrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha, bool useRGB)
	{
		foreach (var target in _targets)
		{
			target.CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha, useRGB);
		}
	}
}
