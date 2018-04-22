using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_EndGameMenu : MonoBehaviour
{
	public Text successFailText;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void SetSuccessFailText( bool bIsSuccess )
	{
		if( successFailText != null )
		{
			successFailText.text = bIsSuccess ? "Congratulations!" : "You were discovered!";
		}
	}

	public void ExitToMain_OnClick()
	{
		if( Core.theCore != null )
		{
			Core.theCore.RequestState(Core.CORE_STATE.START_MENU);
		}
	}

	public void ExitDesktop_OnClick()
	{
		Application.Quit();
	}
}
