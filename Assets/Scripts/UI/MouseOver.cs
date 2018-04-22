using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOver : MonoBehaviour
{
	public string mouseOverText = "";

	private void OnMouseOver()
	{
		UI_HUD.instance.showMouseOver = true;
		UI_HUD.instance.mouseOverText = mouseOverText;
	}

	private void OnMouseExit()
	{
		UI_HUD.instance.showMouseOver = false;
	}
}
