using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using ETTView.UI;

public class CommonPopup : UIViewPopup
{
	[SerializeField] Text _title;
	[SerializeField] Text _text;

	public static async UniTask<CommonPopup> Create(Transform parent, string title, string text)
	{
		var ins = await CreateFromResources<CommonPopup>(parent);
		ins._title.text = title;
		ins._text.text = text;

		return ins;
	}
}
