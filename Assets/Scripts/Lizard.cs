﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lizard : Entity {
    
    public enum State
    {
        IDLE,
        MOVING
    }

    public State state;

    public TileBase currentTile;

    List<KeyValuePair<int, int>> currentPath;

    Transform target;

    TileManager mgr;

    float fSpeed = 1.0f;

	// Use this for initialization
	public override void Start () {
        base.Start();
        mgr = Core.theCore.GetComponent<TileManager>();
	}
	
	// Update is called once per frame
	public override void Update () {
        base.Update();
		if (target != null)
        {
            
            transform.localPosition += Math.Max(Time.deltaTime * fSpeed, (target.position - transform.position).Magnitude) * (target.position - transform.position).Direction;
        }
        if ( (target.position - transform.position).Magnitude < 0.05f)
        {
            target = null;
        }
        switch(state)
        {
            case State.IDLE:
                // TODO - random movement
                break;
            case State.MOVING:
                if (currentPath.Size == 0)
                {
                    state = State.IDLE;
                    break;
                } 
                var next = currentPath[0];
                currentPath.RemoveAt(0);
                target = mgr.tiles[next.Key, next.Value].transform.position;
                break;
        }

	}


}
