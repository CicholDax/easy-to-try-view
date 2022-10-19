using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETTView.Data;

public class ReenactablePrefab : Reenactable
{
	[SerializeField] string _path;

	public string Path => _path;

}
