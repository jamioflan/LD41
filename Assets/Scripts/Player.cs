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

    public List<Queue<Task>> pendingWorkerTasks;


	// Use this for initialization
	void Start ()
	{
		thePlayer = this;
        pendingWorkerTasks = new List<Queue<Task>>();
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
