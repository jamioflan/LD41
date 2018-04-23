using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_EndGameMenu : MonoBehaviour
{
	public Text successFailText;
	public Sprite success, fail;
	public Image newspaper;


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
			successFailText.text = bIsSuccess ? "Mayor " + Player.thePlayer.firstLizard + " sworn in amongst pledges to cease drilling activity in the area" : "Lizardpeople found living under Humantown";
			newspaper.sprite = bIsSuccess ? success : fail;
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
