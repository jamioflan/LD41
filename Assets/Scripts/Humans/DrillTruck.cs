﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillTruck : Entity
{
    private enum Phase
    {
        MOVE,
        UNLOAD,
        DRILL,
    }

    public int iTargetX = 0;
    public int iTargetDepth = 0;
    public float fSpeed = 1.0f;
    private Phase phase = Phase.MOVE;
    public float fTimeUntilNextPhase = 0.0f;
    public Anim unloadAnim, drillAnim;

    public Transform drillPlansPrefab, drillPlansEndPrefab, drillBitPrefab, drillBitEndPrefab;

    // Use this for initialization
    public override void Start ()
    {
        base.Start();
	}
	
	// Update is called once per frame
	public override void Update ()
    {
        base.Update();
        switch(phase)
        {
            case Phase.MOVE:
                if (Mathf.Abs(transform.position.x - (iTargetX + 0.5f)) > 0.1f)
                {
                    // Walk to target
                    transform.localPosition += new Vector3(Time.deltaTime * fSpeed * (bFlip ? -1.0f : 1.0f), 0.0f, 0.0f);
                }
                else
                {
                    transform.localPosition = new Vector3(iTargetX + 0.45f, 0.75f, -3.0f);
                    SetAnim(unloadAnim);
                    fTimeUntilNextPhase = 10.0f;
                    phase = Phase.UNLOAD;
                }
                break;
            case Phase.UNLOAD:
                fTimeUntilNextPhase -= Time.deltaTime;
                if(fTimeUntilNextPhase <= 0.0f)
                {
                    SetAnim(drillAnim);
                    phase = Phase.DRILL;
                    // Redo based on player depth and duration of game so far
                    iTargetDepth = Random.Range(0, 50);

                    // Spawn plans
                    for(int i = 0; i < iTargetDepth; i++)
                    {
                        Transform plans = Instantiate<Transform>(drillPlansPrefab);
                        plans.SetParent(transform);
                        plans.localPosition = new Vector3(0.05f, -1.03125f - i, 0.0f);
                    }

                    Transform end = Instantiate<Transform>(drillPlansEndPrefab);
                    end.SetParent(transform);
                    end.localPosition = new Vector3(0.05f, -1.03125f - iTargetDepth, 0.0f);
                }
                break;
            case Phase.DRILL:

                // Do some drilling
                break;
        }
		
	}
}
