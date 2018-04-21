using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
	static public Core theCore;

	public enum CORE_STATE
	{
		VOID,
		START_MENU,
		IN_GAME,
		PAUSE_MENU,
		END_GAME_MENU,
	}

	public float fGameTimeScale = 0.0f;

	CORE_STATE eCurrentState = CORE_STATE.VOID;
	CORE_STATE eRequestedState = CORE_STATE.VOID;

	public void RequestState( CORE_STATE eState ) { eRequestedState = eState; }

	void Enter_Void() {}
	void Update_Void() {}
	void Exit_Void() {}

	void Enter_StartMenu()
	{
		fGameTimeScale = 0.0f;
	}

	void Update_StartMenu()
	{

	}

	void Exit_StartMenu()
	{

	}

	void Enter_InGame()
	{
		fGameTimeScale = 1.0f;
	}

	void Update_InGame()
	{

	}

	void Exit_InGame()
	{

	}

	void Enter_PauseMenu()
	{
		fGameTimeScale = 0.0f;
	}

	void Update_PauseMenu()
	{

	}

	void Exit_PauseMenu()
	{

	}

	void Enter_EndGameMenu()
	{
		fGameTimeScale = 0.0f;
	}

	void Update_EndGameMenu()
	{

	}

	void Exit_EndGameMenu()
	{

	}

	// Use this for initialization
	void Start ()
    {
		theCore = this;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if( eRequestedState != eCurrentState )
		{
			switch( eCurrentState )
			{
				case CORE_STATE.VOID:
				{
					Exit_Void();
					break;
				}
				case CORE_STATE.START_MENU:
				{
					Exit_StartMenu();
					break;
				}
				case CORE_STATE.IN_GAME:
				{
					Exit_InGame();
					break;
				}
				case CORE_STATE.PAUSE_MENU:
				{
					Exit_PauseMenu();
					break;
				}
				case CORE_STATE.END_GAME_MENU:
				{
					Exit_EndGameMenu();
					break;
				}
				default:
				{
					break;
				}
			}

			switch( eRequestedState )
			{
				case CORE_STATE.VOID:
				{
					Enter_Void();
					break;
				}
				case CORE_STATE.START_MENU:
				{
					Enter_StartMenu();
					break;
				}
				case CORE_STATE.IN_GAME:
				{
					Enter_InGame();
					break;
				}
				case CORE_STATE.PAUSE_MENU:
				{
					Enter_PauseMenu();
					break;
				}
				case CORE_STATE.END_GAME_MENU:
				{
					Enter_EndGameMenu();
					break;
				}
				default:
				{
					break;
				}
			}
		}

		eCurrentState = eRequestedState;

		switch( eCurrentState )
		{
			case CORE_STATE.VOID:
			{
				Update_Void();
				break;
			}
			case CORE_STATE.START_MENU:
			{
				Update_StartMenu();
				break;
			}
			case CORE_STATE.IN_GAME:
			{
				Update_InGame();
				break;
			}
			case CORE_STATE.PAUSE_MENU:
			{
				Update_PauseMenu();
				break;
			}
			case CORE_STATE.END_GAME_MENU:
			{
				Update_EndGameMenu();
				break;
			}
			default:
			{
				break;
			}
		}
	}
}
