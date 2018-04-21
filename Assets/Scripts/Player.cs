using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public static Player thePlayer;

	int metal = 0;
	int gems = 0;
	int mushrooms = 0;
	int lizardsDisguisedAsHumans = 0;

	public struct Task
	{
		string name;
		Sprite icon;
		int currentNumWorkers; // Can be changed by the user
		int maxNumWorkers; // How many are needed - changes according to how many of these tasks are active
	}

	public Task[] tasks;

	// Use this for initialization
	void Start ()
	{
		thePlayer = this;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void Reset()
	{
		metal = 0;
		gems = 0;
		mushrooms = 0;
		lizardsDisguisedAsHumans = 0;
	}
}
