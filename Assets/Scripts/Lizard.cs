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


    private void SetState(State newState)
    {
        Debug.Log("Switching state to " + newState);
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
        for (int i = 0; i < (int)Need.NUM_NEEDS; i++)
            afNeeds[i] -= 0.01f * Time.deltaTime;

        if(ShouldDie())
        {
            TextTicker.AddLine(lizardName + " died of starvation");
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
                            Claim(res);
                            break;
                        }
                    SetPath(clutterPath);
                    break;  
                }
                
                fIdleTime -= Time.deltaTime;
                if (!targetSet) {
                    if (!currentTile.IsPassable())
                    {
                        TileManager.TestTile del = delegate (TileBase tile) { return tile.IsLizardy(); };
                        SetPath(Path.GetPath(currentTile.GetKVPair(), currentTile.GetAdjacentTiles(del)));
                    }
                    else if (fIdleTime < 0)
                        SetTarget(GetTileCenter(currentTile) + new Vector3(Random.Range(-0.35f, 0.35f), 0.0f, 0.0f) );

                }
                else if (Move())
                {
                    // Set a cooldown timer
                    fIdleTime = Random.Range(1.0f, 5.0f);
                }
                break;
            case State.TRAVELLING_TO_TASK:
                if (Move())
                    SetState(State.WORKING);
                break;
            case State.WORKING:
                switch (currentTask.type)
                {
                    case Task.Type.BUILD:
                        currentTask.UseResources();
                        if (currentTile.Build(this))
                        {
                            FinishTask();
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
                    case Task.Type.SELL_RESOURCE:
                        currentTask.UseResources();
                        int value = 0;
                        foreach (KeyValuePair<Resource.ResourceType, int> count in currentTask.requiredResources)
                            value += Player.thePlayer.GetValue(count.Key) * count.Value;
                        Player.thePlayer.money += value;
                        // Maybe put in some animation stuff in time? James?
                        // e.g. transport the resource up to the actual hut
                        SetState(State.IDLE);
                        break;
                    case Task.Type.EAT:
                    case Task.Type.RELAX:
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

                    claimed.GiveToLizard(this);
                    SetState(State.RETURNING_RESOURCE);
                }
                break;
            case State.RETURNING_RESOURCE:
                if (Move() )
                {
                    if (currentTask == null)
                    {
                        Debug.Log("Calling StoreResource");
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

            Debug.Log("Calling GetPath to travel to task");
            if (!SetPath(Path.GetPath(currentTile.GetKVPair(), currentTask.associatedTile.GetKVPair())))
                CannotReachTask();
        }
        else
        {
            // Get the next resource
            var kvs = new List<KeyValuePair<int, int>>();
            foreach (TileBase tile in mgr.GetTilesContaining(nextType))
            {
                Debug.Log(tile);
                kvs.Add(tile.GetKVPair());
            }
            Debug.Log("Calling GetPath to retrieve resource for task");
            if (SetPath(Path.GetPath(currentTile.GetKVPair(), kvs)))
            {
                TileBase targetTile = mgr.GetTileBase(currentPath.endX, currentPath.endY);
                Claim(targetTile.FindResource(nextType));
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

    public void Claim(Resource resource)
    {
        resource.reservee = this;
        if (claimed != null)
        {
            claimed.Unclaim();
        }
        claimed = resource;
        resource.Claim(this);
    }

    public void Take(Resource resource)
    {
        if (carrying != null)
            carrying.Drop();
        if (claimed != resource && claimed != null)
        {
            claimed.Unclaim();
            Core.theTM.RemoveFromUnclaimed(resource);
        }

        claimed = null;
        carrying = resource;
		carrying.transform.SetParent(transform);
		carrying.transform.localPosition = new Vector3(0.0f, 0.0f, -0.1f);

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
                break;
            default:
                TextTicker.AddLine("Should you really be eating that, " + lizardName + "?");
                break;
        }
    }
}
