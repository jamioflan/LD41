using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PauseMenu : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void ExitToMain_OnClick()
	{
		if (Core.theCore != null)
		{
			Core.theCore.RequestState(Core.CORE_STATE.START_MENU);
		}
	}

	public void Unpause()
	{
		Core.theCore.RequestState(Core.CORE_STATE.IN_GAME);
	}

	public void ExitDesktop_OnClick()
	{
		Application.Quit();
	}
}
