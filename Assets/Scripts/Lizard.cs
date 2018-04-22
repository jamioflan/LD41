using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lizard : Entity {
    
    public enum Assignment
    {
        HATCHERY,
        TRAP,
        TAILOR,
        WORKER
    }

    public Assignment assignment = Assignment.WORKER;

    public string lizardName;

    public Task currentTask;

    public enum State
    {
        IDLE,
        TRAVELLING_TO_TASK,
        RETRIEVING_RESOURCE,
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
            SetAnim(idleAnim);
            targetSet = false;
        }
        return reachedTarget;
    }


    private void SetState(State newState)
    {
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
	}

    public void SetTarget(Vector3 newTarget) {
        target = newTarget;
        targetSet = true;
    }
        
	
	// Update is called once per frame
	public override void Update () {
        base.Update();

        switch(state)
        {
            case State.IDLE:
                if (Player.thePlayer.pendingWorkerTasks.Count != 0)
                {
                    currentTask = Player.thePlayer.pendingWorkerTasks[0];
                    Player.thePlayer.pendingWorkerTasks.RemoveAt(0);
                    DoTask();
                    break;
                }
                fIdleTime -= Time.deltaTime;
                if (!targetSet) {
                    if (fIdleTime < 0)
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
                {
                    if (currentPath.Count() == 0)
                    {
                        SetState(State.WORKING);
                    }
                    else
                    {
                        var next = currentPath.Pop();
                        SetTile(mgr.GetTileBase(next.Key, next.Value) );
                        SetTarget(GetTileCenter(mgr.GetTileBase(next.Key, next.Value)) );
                    }
                }
                break;
            case State.WORKING:
                switch (currentTask.type)
                {
                    case Task.Type.BUILD:
                        if (currentTile.Build(this) )
                        {
                            FinishTask();
                            SetState(State.IDLE);
                        }
                        break;
                    case Task.Type.EAT:
                    case Task.Type.RELAX:
                    case Task.Type.WORK_ROOM:
                    case Task.Type.FETCH_RESOURCE:
                        FinishTask();
                        break;
                        
                }
                break;
        }

	}

    public void DoTask()
    {
        Resource.ResourceType nextType = currentTask.GetNextMissing();
        if (nextType == Resource.ResourceType.NULL)
        {
            SetState(State.TRAVELLING_TO_TASK);
            currentPath = Path.GetPath(currentTile.GetKVPair(), currentTask.associatedTile.GetKVPair());
        }
        else
        {
            // Get the next resource
            var kvs = new List<KeyValuePair<int, int>>();
            foreach (TileBase tile in mgr.GetTilesContaining(nextType))
                kvs.Add(tile.GetKVPair());
            currentPath = Path.GetPath(currentTile.GetKVPair(), kvs);
            TileBase targetTile = mgr.GetTileBase(currentPath.endX, currentPath.endY);
            Claim(targetTile.FindResource(nextType));
            SetState(State.RETRIEVING_RESOURCE);
        } 
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
    }

    public void Take(Resource resource)
    {
        if (carrying != null)
            carrying.Drop();
        if (claimed != resource && claimed != null)
            claimed.Unclaim();
        claimed = null;
    }

    public void SetTile(TileBase tile)
    {
        currentTile.lizardsOnTile.Remove(this);
        if (tile != null)
            tile.lizardsOnTile.Add(this);
        currentTile = tile;
    }

}
