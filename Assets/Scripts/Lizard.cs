using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lizard : Entity {
    
    public enum Need
    {
        // Essentials
        FOOD,
        

        // Luxuries - Feed into happiness
        HUMAN_FOOD,
        ENTERTAINMENT,

        NUM_NEEDS,
    }

    public float[] afNeeds = new float[(int)Need.NUM_NEEDS];

    public enum Assignment
    {
        HATCHERY,
        TRAP,
        TAILOR,
        WORKER,
		FARMER,

		NUM_ASSIGNMENTS
	}

    public Assignment assignment = Assignment.WORKER;

    public string lizardName;

    public Task currentTask;

	public AudioClip eatingClip;

    public enum State
    {
        IDLE,
        TRAVELLING_TO_TASK,
        RETRIEVING_RESOURCE,
        RETURNING_RESOURCE,
        WORKING,
    }

    public State state = State.IDLE;

    public TileBase currentTile;

    Path currentPath;

    Vector3 target;
    bool targetSet = false;

    TileManager mgr;

    public float fSpeed = 1.0f;

    // Time left to stand still whilst idling
    public float fIdleTime = 0.0f;

    public Anim idleAnim, walkAnim, climbAnim, interactAnim;

    public Resource claimed;
    public Resource carrying;

    public void Destroy()
    {
        mgr.lizards[assignment].Remove(this);
        SetTile(null);
        Destroy(gameObject);
    }

    // Get the position a lizard should be to be at the center of a tile
    public static Vector3 GetTileCenter(TileBase tile)
    {
        return tile.transform.position + new Vector3(0.0f, 0.05f, -1.0f);
    }

    // Move towards the current target
    // Returns true if the target has been reached
    private bool Move()
    {
        if (!targetSet)
            return true;
        float tolerance = 0.01f;
        Vector3 difference = target - transform.position;
        bool reachedTarget;
        if (Mathf.Abs(difference.x) > Mathf.Abs(difference.y) )
        {
            float distance = difference.x;
            SetAnim(walkAnim);
            SetLeft(distance > 0);
            transform.localPosition += new Vector3(Mathf.Sign(distance), 0.0f, 0.0f) * Mathf.Min(Mathf.Abs(distance), Time.deltaTime * fSpeed);
            reachedTarget = Mathf.Abs((target - transform.position).x) < tolerance;
        }
        else
        {
            float distance = difference.y;
            SetAnim(climbAnim);
            transform.localPosition += new Vector3(0.0f, Mathf.Sign(distance), 0.0f) * Mathf.Min(Mathf.Abs(distance), Time.deltaTime * fSpeed);
            reachedTarget = Mathf.Abs((target - transform.position).y) < tolerance;
        }
        if (reachedTarget)
        {
            if (currentPath.Count() == 0)
            {
                SetAnim(idleAnim);
                targetSet = false;
                return true;
            }
            else
            {
                var next = currentPath.Pop();
                SetTile(mgr.GetTileBase(next.Key, next.Value));
                SetTarget(GetTileCenter(mgr.GetTileBase(next.Key, next.Value)));
                return false;
            }
        }
        return reachedTarget;
    }


    public void SetState(State newState)
    {
        //Debug.Log("Switching state to " + newState);
        state = newState;
        switch(newState)
        {
            case State.IDLE:
                SetAnim(idleAnim);
                break;
            case State.TRAVELLING_TO_TASK:
                SetAnim(walkAnim);
                break;
            case State.WORKING:
                SetAnim(interactAnim);
                break;
        }
    }

	// Use this for initialization
	public override void Start () {
        base.Start();
        mgr = Core.theCore.GetComponent<TileManager>();
        currentPath = new Path();

        for (int i = 0; i < (int)Need.NUM_NEEDS; i++)
            afNeeds[i] = 1.0f;
        currentTask = null;
	}

    public void SetTarget(Vector3 newTarget) {
        target = newTarget;
        targetSet = true;
    }
        
	
	// Update is called once per frame
	public override void Update () {
        base.Update();
		bool bWasInDangerOfStarving = AmInDangerOfStarving();
		bool bWasInDangerOfBreaking = AmInDangerOfBreaking();

		afNeeds[(int)Need.FOOD] -= 0.005f * Time.deltaTime;
		afNeeds[(int)Need.HUMAN_FOOD] -= 0.0025f * Time.deltaTime;
		afNeeds[(int)Need.ENTERTAINMENT] -= 0.0025f * Time.deltaTime;

		if(!bWasInDangerOfStarving && AmInDangerOfStarving())
		{
			TextTicker.AddLine(lizardName + " is starving. Get them some food");
			if (state != State.IDLE)
				SetState(State.IDLE);
		}
		if (!bWasInDangerOfBreaking && AmInDangerOfBreaking())
		{
			TextTicker.AddLine(lizardName + " is going mad. Get them some TV or fancy human food");
			if (state != State.IDLE)
				SetState(State.IDLE);
		}

		if (ShouldDie())
        {
            TextTicker.AddLine(lizardName + " died of starvation");
			Core.theTM.lizards[assignment].Remove(this);
            Destroy(gameObject);
        }

        if (ShouldGoMad())
        {
            TextTicker.AddLine(lizardName + " has gone mad and is trying to leave");
            // TODO: Go into escape state
        }

        switch (state)
        {
            case State.IDLE:
				if (ShouldEat())
				{
					// Check to see if there are reachable storerooms
					TileManager.TestTile del = delegate (TileBase tile)
					{
						return tile.FindResource(Resource.ResourceType.MUSHROOMS) != null || tile.FindResource(Resource.ResourceType.HUMAN_FOOD) != null;
					};

					var pathToFoods = Path.GetPath(currentTile.GetKVPair(), mgr.GetTiles(del));
					if (pathToFoods != null)
					{
						TileBase targetTile = mgr.GetTileBase(pathToFoods.endX, pathToFoods.endY);
						foreach (Resource res in targetTile.clutteredResources)
						{
							if (!res.isClaimed && (res.type == Resource.ResourceType.MUSHROOMS || res.type == Resource.ResourceType.HUMAN_FOOD))
							{
								res.Claim(this);
								break;
							}
						}
						if (claimed == null)
						{
							foreach (Resource res in targetTile.tidyResources)
							{
								if (res != null && !res.isClaimed && (res.type == Resource.ResourceType.MUSHROOMS || res.type == Resource.ResourceType.HUMAN_FOOD))
								{
									res.Claim(this);
									break;
								}
							}
						}

						if (claimed != null)
						{
							SetState(State.RETRIEVING_RESOURCE);
							currentTask = new Task(Task.Type.EAT);
							SetPath(pathToFoods);
							break;
						}
					}
				}
				if (ShouldFindEntertainment())
				{
					// Check to see if there are reachable storerooms
					TileManager.TestTile del = delegate (TileBase tile)
					{
						return tile.Type() == TileBase.TileType.TVROOM;
					};

					var pathToTV = Path.GetPath(currentTile.GetKVPair(), mgr.GetTiles(del));
					if (pathToTV != null)
					{
						TileBase targetTile = mgr.GetTileBase(pathToTV.endX, pathToTV.endY);

						SetState(State.TRAVELLING_TO_TASK);
						currentTask = new Task(Task.Type.RELAX);
						currentTask.associatedTile = targetTile;
						currentTask.assignedLizard = this;

						SetPath(pathToTV);
						break;
					}
				}
				if (!AmInDangerOfStarving() && !AmInDangerOfBreaking())
				{
					if (Player.thePlayer.pendingTasks[(int)assignment].Count != 0)
					{
						foreach (Task task in Player.thePlayer.pendingTasks[(int)assignment])
						{
							// Check to see if this lizard can reach the task
							if (Path.GetPath(currentTile.GetKVPair(), task.associatedTile) != null)
							{
								currentTask = task;
								break;
							}
						}
						if (currentTask != null)
						{
							currentTask.assignedLizard = this;
							Player.thePlayer.pendingTasks[(int)assignment].Remove(currentTask);
							DoTask();
						}

						break;
					}
					//Debug.Log("Calling GetPath to find dropped resources!");
					var clutterPath = Path.GetPath(currentTile.GetKVPair(), mgr.GetClutteredTiles());
					if (clutterPath != null)
					{
						// Check to see if there are reachable storerooms
						TileManager.TestTile del = delegate (TileBase tile)
						{
							return tile.Type() == TileBase.TileType.STORAGE && tile.NEmptyResourceSlots() > 0;
						};
						Debug.Log("Calling GetPath to check for a store room");
						var storeroomPath = Path.GetPath(currentTile.GetKVPair(), mgr.GetTiles(del));
						if (storeroomPath == null)
							break;
						SetState(State.RETRIEVING_RESOURCE);
						TileBase targetTile = mgr.GetTileBase(clutterPath.endX, clutterPath.endY);
						foreach (Resource res in targetTile.clutteredResources)
							if (!res.isClaimed)
							{
								res.Claim(this);
								break;
							}
						SetPath(clutterPath);
						break;
					}

					fIdleTime -= Time.deltaTime;
					if (!targetSet)
					{
						if (!currentTile.IsPassable())
						{
							TileManager.TestTile del = delegate (TileBase tile) { return tile.IsLizardy(); };
							SetPath(Path.GetPath(currentTile.GetKVPair(), currentTile.GetAdjacentTiles(del)));
						}
						else if (fIdleTime < 0)
							SetTarget(GetTileCenter(currentTile) + new Vector3(Random.Range(-0.35f, 0.35f), 0.0f, 0.0f));

					}
					else if (Move())
					{
						// Set a cooldown timer
						fIdleTime = Random.Range(1.0f, 5.0f);
					}
				}
                break;
            case State.TRAVELLING_TO_TASK:
				if (Move())
				{
					if(currentTask.associatedTile != null)
						currentTask.associatedTile.SetTaskActive(true);
					SetState(State.WORKING);
				}
                break;
            case State.WORKING:
                switch (currentTask.type)
                {
                    case Task.Type.BUILD:
                        currentTask.UseResources();
                        if (currentTile.Build(this))
                        {
                            FinishTask();
							UI_HUD.instance.PlansFinished(currentTile.x, currentTile.y);

							if (currentTile.Type() == TileBase.TileType.FILLED)
                            {

                            }
                            SetState(State.IDLE);
                        }
                        break;
                    case Task.Type.BREED:
                        if (currentTile is Hatchery)
                        {
                            if ((currentTile as Hatchery).Breed(this))
                            {
                                FinishTask();
                                SetState(State.IDLE);
                            }
                        }
                        break;
					case Task.Type.FARM:
						if(currentTile is MushroomFarm)
						{
							if((currentTile as MushroomFarm).Farm())
							{
								FinishTask();
								SetState(State.IDLE);
							}
						}
						break;
					case Task.Type.TRAP:
						if (currentTile is Trap)
						{
							if ((currentTile as Trap).Farm())
							{
								FinishTask();
								SetState(State.IDLE);
							}
						}
						break;
					case Task.Type.TAILOR:
						if (currentTile is Tailor)
						{
							if ((currentTile as Tailor).Farm())
							{
								FinishTask();
								SetState(State.IDLE);
							}
						}
						break;
					case Task.Type.RELAX:
						if(currentTile is TVRoom)
						{
							afNeeds[(int)Need.ENTERTAINMENT] += 0.1f * Time.deltaTime;
							(currentTile as TVRoom).IncrementTVBill(0.3f * Time.deltaTime);
							if(afNeeds[(int)Need.ENTERTAINMENT] > 0.9f)
							{
								FinishTask();
								SetState(State.IDLE);
							}
						}
						break;
                    case Task.Type.SELL_RESOURCE:
                        currentTask.UseResources();
                        int value = 0;
                        foreach (KeyValuePair<Resource.ResourceType, int> count in currentTask.requiredResources)
                            value += Player.thePlayer.GetValue(count.Key) * count.Value;
                        Player.thePlayer.money += value;
						FinishTask();
                        // Maybe put in some animation stuff in time? James?
                        // e.g. transport the resource up to the actual hut
                        SetState(State.IDLE);
                        break;
                    case Task.Type.EAT:
                    case Task.Type.WORK_ROOM:
                        FinishTask();
                        break;
                        
                }
                break;
            case State.RETRIEVING_RESOURCE:
                if(Move() )
                {
                    if (currentTask == null)
                    {
                        // Find a storeroom with an empty slot
                        TileManager.TestTile del = delegate (TileBase tile)
                        {
                            return tile.Type() == TileBase.TileType.STORAGE && tile.NEmptyResourceSlots() > 0;
                        };
                        Debug.Log("Calling GetPath to go to a storeroom");
                        var storePath = Path.GetPath(currentTile.GetKVPair(), mgr.GetTiles(del));
                        if (storePath == null)
                        {
                            SetState(State.IDLE);
                            break;
                        }
                        else
                        {
                            SetPath(storePath);
                        }
                    }
					else if(currentTask.associatedTile == null)
					{
						// No tile. Must be a self task
						switch(currentTask.type)
						{
							case Task.Type.EAT:
								Consume(claimed);
								break;
							default:
								Debug.Assert(false, "This task should have an associated tile");
								break;
						}
						SetState(State.IDLE);
						return;
					}
                    else
                    {
                        Debug.Log("Calling GetPath to go to the target tile");
                        var tilePath = Path.GetPath(currentTile.GetKVPair(), currentTask.associatedTile);
                        if (tilePath == null)
                        {
                            CannotReachTask();
                        }
                        else
                        {
                            SetPath(tilePath);
                        }
                    }

					if (claimed != null)
					{
						claimed.GiveToLizard(this);
						SetState(State.RETURNING_RESOURCE);
					}
					else
					{
						// Someone nicked it!
						SetState(State.IDLE);
					}
                }
                break;
            case State.RETURNING_RESOURCE:
                if (Move() )
                {
                    if (currentTask == null)
                    {
                        //Debug.Log("Calling StoreResource");
                        carrying.PutInRoom(currentTile);

                        SetState(State.IDLE);
                    }
                    else
                    {
                        currentTask.AddResource(carrying);
                        DoTask();
                    }
                }
                break;
        }

	}

    public void CannotReachTask()
    {
        // Need to give up on this task
        currentTask.assignedLizard = null;
        Player.thePlayer.pendingTasks[(int)assignment].Add(currentTask);
        currentTask = null;
        SetState(State.IDLE);
    }

    public void DoTask()
    {
        Resource.ResourceType nextType = currentTask.GetNextMissing();
        if (nextType == Resource.ResourceType.NULL)
        {
            SetState(State.TRAVELLING_TO_TASK);

            //Debug.Log("Calling GetPath to travel to task");
            if (!SetPath(Path.GetPath(currentTile.GetKVPair(), currentTask.associatedTile.GetKVPair())))
                CannotReachTask();
        }
        else
        {
            // Get the next resource
            var kvs = new List<KeyValuePair<int, int>>();
            foreach (TileBase tile in mgr.GetTilesContaining(nextType))
            {
                //Debug.Log(tile);
                kvs.Add(tile.GetKVPair());
            }
            //Debug.Log("Calling GetPath to retrieve resource for task");
            if (SetPath(Path.GetPath(currentTile.GetKVPair(), kvs)))
            {
                TileBase targetTile = mgr.GetTileBase(currentPath.endX, currentPath.endY);
				targetTile.FindResource(nextType).Claim(this);
                SetState(State.RETRIEVING_RESOURCE);
            }
            else
            {
                CannotReachTask();
            }
        } 
    }

    public bool SetPath(Path path)
    {
        if (path == null)
        {
            targetSet = false;
            return false;
        }
        currentPath = path;
        var next = currentPath.Pop();
        SetTile(mgr.GetTileBase(next.Key, next.Value));
        SetTarget(GetTileCenter(mgr.GetTileBase(next.Key, next.Value)));
        return true;
    }

    public void FinishTask()
    {
        currentTask.Finish();
    }

    public void SetTile(TileBase tile)
    {
        if (currentTile != null)
            currentTile.lizardsOnTile.Remove(this);
        if (tile != null)
            tile.lizardsOnTile.Add(this);
        currentTile = tile;
    }

    public bool ShouldDie() { return afNeeds[(int)Need.FOOD] <= 0.0f; }
    public bool ShouldGoMad() { return afNeeds[(int)Need.HUMAN_FOOD] + afNeeds[(int)Need.ENTERTAINMENT] <= 0.0f; }

	public bool ShouldEat() { return afNeeds[(int)Need.FOOD] <= 0.5f; }
	public bool ShouldFindEntertainment() { return afNeeds[(int)Need.HUMAN_FOOD] + afNeeds[(int)Need.ENTERTAINMENT] <= 0.5f; }

	public bool AmInDangerOfStarving()
    {
        return afNeeds[(int)Need.FOOD] < 0.25f;
    }

    public bool AmInDangerOfBreaking()
    { 
        return afNeeds[(int)Need.HUMAN_FOOD] + afNeeds[(int)Need.ENTERTAINMENT] < 0.25f;
    }

    public void Consume(Resource resource)
    {
        switch(resource.type)
        {
            case Resource.ResourceType.MUSHROOMS:
                afNeeds[(int)Need.FOOD] += 0.5f;
                break;
            case Resource.ResourceType.HUMAN_FOOD:
                afNeeds[(int)Need.HUMAN_FOOD] += 0.5f;
				afNeeds[(int)Need.FOOD] += 0.5f;
				break;
            default:
                TextTicker.AddLine("Should you really be eating that, " + lizardName + "?");
                break;
        }

		GetComponent<AudioSource>().clip = eatingClip;
		GetComponent<AudioSource>().Play();
		fDelay = Random.Range(fMinDelay, fMaxDelay);

		resource.Destroy();
		SetState(State.IDLE);
		currentTask = null;
    }
}
