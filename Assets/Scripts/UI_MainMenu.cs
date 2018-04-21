using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MainMenu : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void Start_OnClick()
	{
		if (Core.theCore != null)
		{
			Core.theCore.RequestState(Core.CORE_STATE.IN_GAME);
		}
	}
}
