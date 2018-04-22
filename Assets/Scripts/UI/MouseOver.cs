using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOver : MonoBehaviour
{
	public string mouseOverText = "";

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

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
