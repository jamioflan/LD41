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

    public string name;

    public Queue<Task> taskList = new Queue<Task>();

    public enum State
    {
        IDLE,
        TRAVELLING_TO_TASK,
    }

    public State state = State.IDLE;

    public TileBase currentTile;

    List<KeyValuePair<int, int>> currentPath;

    Vector3 target;
    bool targetSet = false;

    TileManager mgr;

    public float fSpeed = 1.0f;

    // Time left to stand still whilst idling
    public float fIdleTime = 0.0f;

    public Anim idleAnim, walkAnim, climbAnim, interactAnim;


    public void Destroy()
    {
        int idx = mgr.lizards.IndexOf(this);
        mgr.lizards.RemoveAt(idx);
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
            transform.localPosition += new Vector3(Mathf.Sign(distance), 0.0f, 0.0f) * Mathf.Max(distance, Time.deltaTime * fSpeed);
            reachedTarget = Mathf.Abs((target - transform.position).x) < tolerance;
        }
        else
        {
            float distance = difference.y;
            SetAnim(climbAnim);
            transform.localPosition += new Vector3(0.0f, Mathf.Sign(distance), 0.0f) * Mathf.Max(distance, Time.deltaTime * fSpeed);
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
        }
    }

	// Use this for initialization
	public override void Start () {
        base.Start();
        mgr = Core.theCore.GetComponent<TileManager>();
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
                if (currentPath.Count == 0)           
                {
                    SetState(State.IDLE);
                    break;
                } 
                var next = currentPath[0];
                currentPath.RemoveAt(0);
                target = GetTileCenter(mgr.tiles[next.Key, next.Value]);
                break;
        }

	}


}
