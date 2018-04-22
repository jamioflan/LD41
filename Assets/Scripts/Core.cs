using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour
{
	static public Core theCore;
    public static TileManager theTM
    {
        get { return theCore.GetComponent<TileManager>(); }
    }

	public enum CORE_STATE
	{
		VOID,
		START_MENU,
		IN_GAME,
		PAUSE_MENU,
		END_GAME_MENU,
	}

	public float cameraMaxY = 4.0f;
	public float cameraMinY = -15.0f;

	public GameObject mainMenu;
	public GameObject pauseMenu;
	public GameObject HUD;
	public GameObject endGameMenu;

	CORE_STATE eCurrentState = CORE_STATE.VOID;
	CORE_STATE eRequestedState = CORE_STATE.VOID;

	bool bWasEscPressed = false;
	bool bIsEscPressed = false;

	bool bIsSuccess = false;

	public void RequestState(CORE_STATE eState) { eRequestedState = eState; }

	public void SetIsSuccess(bool bSuccess) { bIsSuccess = bSuccess; }

	void Reset()
	{
		// Logic to perform on resetting the game (called in Enter_StartMenu)
		bIsSuccess = false;

		if (Player.thePlayer != null)
		{
			Player.thePlayer.Reset();
		}

		TileManager theTileManager = Core.theCore.GetComponent<TileManager>();
		if (theTileManager != null)
		{
			theTileManager.Reset();
		}

		Camera.main.transform.position = new Vector3(0.0f, 1.0f, -10.0f);

		// Reset all menus to default state
		if (mainMenu != null)
		{
			mainMenu.SetActive(true);
		}

		if (pauseMenu != null)
		{
			pauseMenu.SetActive(false);
		}

		if (endGameMenu != null)
		{
			endGameMenu.SetActive(false);
		}

		if (HUD != null)
		{
			HUD.SetActive(false);
		}

		UI_HUD HUDClass = GetComponent<UI_HUD>();
		if (HUDClass != null)
		{
			HUDClass.Reset();
		}
	}

	void Enter_Void() {}
	void Update_Void() {}
	void Exit_Void() {}

	void Enter_StartMenu()
	{
		Time.timeScale = 0.0f;

		Reset();
	}

	void Update_StartMenu()
	{

	}

	void Exit_StartMenu()
	{
		if (mainMenu != null)
		{
			mainMenu.SetActive(false);
		}
	}

	void Enter_InGame()
	{
		Time.timeScale = 1.0f;

		if (HUD != null)
		{
			HUD.SetActive(true);
		}
	}

	void Update_InGame()
	{
		if( !bWasEscPressed && bIsEscPressed )
		{
			RequestState(CORE_STATE.PAUSE_MENU);
		}

		if( Input.GetAxis("Mouse ScrollWheel") > 0 )
		{
			float fNewY = Mathf.Min(Camera.main.transform.position.y + 1.0f, cameraMaxY);
			Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, fNewY, Camera.main.transform.position.z );
		}
		else if (Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			float fNewY = Mathf.Max(Camera.main.transform.position.y - 1.0f, cameraMinY);
			Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, fNewY, Camera.main.transform.position.z);
		}
	}

	void Exit_InGame()
	{
		if (HUD != null)
		{
			HUD.SetActive(false);
		}
	}

	void Enter_PauseMenu()
	{
		Time.timeScale = 0.0f;

		if (pauseMenu != null)
		{
			pauseMenu.SetActive(true);
		}
	}

	void Update_PauseMenu()
	{
		if (!bWasEscPressed && bIsEscPressed)
		{
			RequestState(CORE_STATE.IN_GAME);
		}
	}

	void Exit_PauseMenu()
	{
		if (pauseMenu != null)
		{
			pauseMenu.SetActive(false);
		}
	}

	void Enter_EndGameMenu()
	{
		Time.timeScale = 0.0f;

		if (endGameMenu != null)
		{
			endGameMenu.SetActive(true);

			UI_EndGameMenu menuClass = endGameMenu.GetComponent<UI_EndGameMenu>();
			if( menuClass != null )
			{
				menuClass.SetSuccessFailText(bIsSuccess);
			}
		}
	}

	void Update_EndGameMenu()
	{

	}

	void Exit_EndGameMenu()
	{
		if (endGameMenu != null)
		{
			endGameMenu.SetActive(false);
		}
	}

	// Use this for initialization
	void Start ()
    {
		theCore = this;
		RequestState(CORE_STATE.START_MENU);
	}
	
	// Update is called once per frame
	void Update ()
    {
		bIsEscPressed = ( Input.GetAxisRaw("Cancel") > 0.0f );

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

		bWasEscPressed = bIsEscPressed;
	}

    public void Win()
    {
		RequestState(CORE_STATE.END_GAME_MENU);
		bIsSuccess = true;
    }

    public void Lose()
    {
		RequestState(CORE_STATE.END_GAME_MENU);
		bIsSuccess = false;
	}
}
