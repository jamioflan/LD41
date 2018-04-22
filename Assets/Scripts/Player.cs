using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public static Player thePlayer;

	public int metal = 0;
	public int gems = 0;
	public int mushrooms = 0;

	public int lizardsDisguisedAsHumans = 0;
    public float fHumanSuspicion = 0.0f;

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
        fHumanSuspicion -= 0.1f * Time.deltaTime;
        if(fHumanSuspicion >= 100.0f)
        {
            Core.theCore.Lose();
        }

        if(lizardsDisguisedAsHumans >= 10)
        {
            Core.theCore.Win();
        }
	}

	public void Reset()
	{
		metal = 0;
		gems = 0;
		mushrooms = 0;
		lizardsDisguisedAsHumans = 0;
	}

    public void AddSuspicion(float fSusp)
    {
        fHumanSuspicion += fSusp;
    }
}
