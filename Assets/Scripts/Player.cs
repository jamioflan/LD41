using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public static Player thePlayer;

	private static readonly int HUMANFOOD_BUY_PRICE = 3;

	public string firstLizard = "Boris";

	public float fTimeToNextRandomEvent = 60.0f;

	public int metal = 0;
	public int gems = 0;
	public int mushrooms = 0;
	public int dinosaurBones = 0;
    public int humanFood = 0;
    public int humanSkins = 0;
	public int money = 0;

	public int metalValue = 5;
	public int gemValue = 10;
	public int mushroomValue = 1;
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

	public Resource humanFoodPrefab;

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

		if(fHumanSuspicion > 0.0f && Core.theTM.GetNumBreeders() + Core.theTM.GetNumFarmers() + Core.theTM.GetNumMisc() + Core.theTM.GetNumTailors() + Core.theTM.GetNumTrappers() == 0)
		{
			Core.theCore.Lose();
		}

        if(lizardsDisguisedAsHumans >= 3)
        {
            Core.theCore.Win();
        }

		fTimeToNextRandomEvent -= Time.deltaTime;
		if(fTimeToNextRandomEvent <= 0.0f)
		{
			fTimeToNextRandomEvent = Random.Range(30.0f, 120.0f);
			List<Lizard> list = GetAllLizards();
			int pick1 = Random.Range(0, list.Count);
			int pick2 = Random.Range(0, list.Count - 1);
			if (pick2 == pick1)
				pick2 = list.Count - 1;
			switch (Random.Range(0, 5))
			{
				case 0:
					TextTicker.AddLine("<color=purple>" + list[pick1].lizardName + " and " + list[pick2].lizardName + " had a fight</color>");
					TextTicker.AddLine("<color=purple>They are both upset by this</color>");
					list[pick1].afNeeds[(int)Lizard.Need.ENTERTAINMENT] = Mathf.Clamp(list[pick1].afNeeds[(int)Lizard.Need.ENTERTAINMENT] - 0.5f, 0.2f, 1.0f);
					list[pick2].afNeeds[(int)Lizard.Need.ENTERTAINMENT] = Mathf.Clamp(list[pick2].afNeeds[(int)Lizard.Need.ENTERTAINMENT] - 0.5f, 0.2f, 1.0f);
					break;
				case 1:
					TextTicker.AddLine("<color=purple>" + list[pick1].lizardName + " found an extra tin of human food</color>");
					TextTicker.AddLine("<color=purple>They are now full of tasty food and are feeling happy</color>");
					list[pick1].afNeeds[(int)Lizard.Need.ENTERTAINMENT] = 1.0f;
					list[pick1].afNeeds[(int)Lizard.Need.HUMAN_FOOD] = 1.0f;
					break;
				case 2:
					string newName = Random.Range(0, 2) == 0 ? "Steve" : "Loretta";
					TextTicker.AddLine("<color=purple>" + list[pick1].lizardName + " has decided to change their name to " + newName + "</color>");
					list[pick1].lizardName = newName;
					list[pick1].afNeeds[(int)Lizard.Need.ENTERTAINMENT] = 1.0f;
					break;
				case 3:
					TextTicker.AddLine("<color=purple>" + list[pick1].lizardName + " has the munchies</color>");
					list[pick1].afNeeds[(int)Lizard.Need.FOOD] = 0.1f;
					break;
				case 4:
					TextTicker.AddLine("<color=purple>" + list[pick1].lizardName + " and " + list[pick2].lizardName + " had a romantic moment</color>");
					TextTicker.AddLine("<color=purple>They're both in a great mood</color>");
					list[pick1].afNeeds[(int)Lizard.Need.FOOD] = 1.0f;
					list[pick1].afNeeds[(int)Lizard.Need.ENTERTAINMENT] = 1.0f;
					list[pick1].afNeeds[(int)Lizard.Need.HUMAN_FOOD] = 1.0f;
					list[pick2].afNeeds[(int)Lizard.Need.FOOD] = 1.0f;
					list[pick2].afNeeds[(int)Lizard.Need.ENTERTAINMENT] = 1.0f;
					list[pick2].afNeeds[(int)Lizard.Need.HUMAN_FOOD] = 1.0f;
					break;
			}
		}
	}

	public List<Lizard> GetAllLizards()
	{
		List<Lizard> list = new List<Lizard>();

		foreach (List<Lizard> lizList in Core.theTM.lizards.Values)
		{
			foreach (Lizard lizard in lizList)
			{
				list.Add(lizard);
			}
		}
		return list;
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

	public void PurgeTasks(int x, int y)
	{
		foreach (List<Lizard> lizList in Core.theTM.lizards.Values)
		{
			foreach (Lizard lizard in lizList)
			{
				if (lizard.currentTask != null
					&& lizard.currentTask.associatedTile != null
					&& lizard.currentTask.associatedTile.x == x
					&& lizard.currentTask.associatedTile.y == y)
				{
					lizard.FinishTask();
					lizard.SetState(Lizard.State.IDLE);
				}
			}
		}
		for (int i = 0; i < pendingTasks.Length; i++)
		{
			for (int j = 0; j < pendingTasks[i].Count; j++)
			{
                if (pendingTasks[i][j].associatedTile != null
                    && pendingTasks[i][j].associatedTile.x == x
                    && pendingTasks[i][j].associatedTile.y == y)
                {
                    pendingTasks[i].RemoveAt(j);
                    return;
                }
			}
		}
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

	public void BuyHumanFood(int iNumToBuy)
	{
		iNumToBuy = Mathf.Clamp(iNumToBuy, 0, money / HUMANFOOD_BUY_PRICE);
		for (int ii = 0; ii < iNumToBuy; ++ii)
		{
			money -= HUMANFOOD_BUY_PRICE;
			Resource food = Instantiate<Resource>(humanFoodPrefab);
			food.transform.position = transform.position + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), -1.0f);
			food.PutInRoom(Core.theTM.hutTile);
			Core.theTM.RegisterResource(food);
		}
	}


	public void IncrementResourceCount(Resource.ResourceType type, int amount = 1)
    {
        //Debug.Log("IncrementResourceCount(" + type + ", " + amount + ")");
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
