using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public static Player thePlayer;

	public int metal = 0;
	public int gems = 0;
	public int mushrooms = 0;
	public int dinosaurBones = 0;
    public int humanFood = 0;
    public int humanSkins = 0;
	public int money = 0;

	public int metalValue = 5;
	public int gemValue = 10;
	public int mushroomValue = 2;
    public int GetValue(Resource.ResourceType type)
    {
        switch (type)
        {
            case Resource.ResourceType.METAL:
                return metalValue;
            case Resource.ResourceType.GEMS:
                return gemValue;
            case Resource.ResourceType.MUSHROOMS:
                return mushroomValue;
        }
        return 0;

    }

	public int lizardsDisguisedAsHumans = 0;
    public float fHumanSuspicion = 0.0f;

	public List<Task>[] pendingTasks = new List<Task>[(int)Lizard.Assignment.NUM_ASSIGNMENTS];
   
    // Use this for initialization
    void Start ()
	{
		thePlayer = this;
		for(int i = 0; i < (int)Lizard.Assignment.NUM_ASSIGNMENTS; i++)
		{
			pendingTasks[i] = new List<Task>();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
        fHumanSuspicion -= 0.1f * Time.deltaTime;
		if( fHumanSuspicion < 0.0f )
		{
			fHumanSuspicion = 0.0f;
		}
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
		dinosaurBones = 0;
		money = 0;
		lizardsDisguisedAsHumans = 0;
	}

    public void AddSuspicion(float fSusp)
    {
        fHumanSuspicion += fSusp;
    }

	public void SellMetal( int iNumToSell )
	{
        iNumToSell = Mathf.Clamp(iNumToSell, 0, metal);
        for (int ii = 0; ii < iNumToSell; ++ ii)
        {
            Task task = new Task(Task.Type.SELL_RESOURCE, new Dictionary<Resource.ResourceType, int>() { { Resource.ResourceType.METAL, 1} });
            task.associatedTile = Core.theTM.hutTile;
            pendingTasks[(int)Lizard.Assignment.WORKER].Add(task);
        }

	}

	public void SellGems(int iNumToSell)
	{
        iNumToSell = Mathf.Clamp(iNumToSell, 0, gems);
        for (int ii = 0; ii < iNumToSell; ++ii)
        {
            Task task = new Task(Task.Type.SELL_RESOURCE, new Dictionary<Resource.ResourceType, int>() { { Resource.ResourceType.GEMS, 1 } });
            task.associatedTile = Core.theTM.hutTile;
            pendingTasks[(int)Lizard.Assignment.WORKER].Add(task);
        }
	}

	public void SellMushrooms(int iNumToSell)
	{
		iNumToSell = Mathf.Clamp(iNumToSell, 0, mushrooms);
        for (int ii = 0; ii < iNumToSell; ++ii)
        {
            Task task = new Task(Task.Type.SELL_RESOURCE, new Dictionary<Resource.ResourceType, int>() { { Resource.ResourceType.MUSHROOMS, 1 } });
            task.associatedTile = Core.theTM.hutTile;
            pendingTasks[(int)Lizard.Assignment.WORKER].Add(task);
        }
	}

    public void IncrementResourceCount(Resource.ResourceType type, int amount = 1)
    {
        Debug.Log("IncrementResourceCount(" + type + ", " + amount + ")");
        switch (type)
        {
            case Resource.ResourceType.METAL:
                metal += amount;
                break;
            case Resource.ResourceType.GEMS:
                gems += amount;
                break;
            case Resource.ResourceType.MUSHROOMS:
                mushrooms += amount;
                break;
            case Resource.ResourceType.HUMAN_FOOD:
                humanFood += amount;
                break;
            case Resource.ResourceType.HUMAN_SKIN:
                humanSkins += amount;
                break;
            case Resource.ResourceType.BONES:
                dinosaurBones += amount;
                break;
        }
    }
}
