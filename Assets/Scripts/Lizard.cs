using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lizard : Entity {
    
    public enum State
    {
        IDLE,
        TRAVELLING_TO_TASK,
    }

    public State state;

    public TileBase currentTile;

    List<KeyValuePair<int, int>> currentPath;

    Transform target;

    TileManager mgr;

    float fSpeed = 1.0f;



    public void Destroy()
    {
        int idx = mgr.lizards.IndexOf(this);
        mgr.lizards.RemoveAt(idx);
        Destroy(gameObject);
    }

	// Use this for initialization
	public override void Start () {
        base.Start();
        mgr = Core.theCore.GetComponent<TileManager>();
	}
	
	// Update is called once per frame
	public override void Update () {
        //base.Update();
        if (target != null)
        {
            var targetPosition = target.position - new Vector3(0.0f, 0.0f, 1.0f);
            transform.localPosition += Mathf.Max(Time.deltaTime * fSpeed, (targetPosition - transform.position).magnitude) * (targetPosition - transform.position).normalized;

            if ((targetPosition - transform.position).magnitude < 0.01f)
            {
                target = null;
            }
        }
        switch(state)
        {
            case State.IDLE:
                // TODO - random movement
                break;
            case State.MOVING:
                if (currentPath.Count == 0)           
                {
                    state = State.IDLE;
                    break;
                } 
                var next = currentPath[0];
                currentPath.RemoveAt(0);
                target = mgr.tiles[next.Key, next.Value].transform;
                break;
        }

	}


}
