using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillTruck : Entity
{
    private enum Phase
    {
        MOVE,
        UNLOAD,
        DRILL,
        LEAVE,
    }

    public int iTargetX = 0;
    public int iTargetDepth = 0;
    public float fSpeed = 1.0f, fDrillSpeed = 0.1f;
    private Phase phase = Phase.MOVE;
    public float fTimeUntilNextPhase = 0.0f;
    public Anim walkAnim, unloadAnim, drillAnim;
    public float fDrillProgress = 0.0f;

    public Transform drillPlansPrefab, drillPlansEndPrefab, drillBitEndPrefab;
    public SpriteRenderer drillBitPrefab;

    public SpriteRenderer topDrill;
    public List<SpriteRenderer> drills = new List<SpriteRenderer>();
    public Transform drillBit;
    public List<Transform> drillPlans = new List<Transform>();

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
                if (Mathf.Abs(transform.position.x - (iTargetX - TileManager.width / 2 + 0.5f)) > 0.1f)
                {
                    // Walk to target
                    transform.localPosition += new Vector3(Time.deltaTime * fSpeed * (bFlip ? -1.0f : 1.0f), 0.0f, 0.0f);
                }
                else
                {
                    transform.localPosition = new Vector3(iTargetX - TileManager.width / 2 + 0.45f, 0.75f, -3.0f);
                    SetAnim(unloadAnim);
                    fTimeUntilNextPhase = 5.0f;
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
                    iTargetDepth = Random.Range(0, TileManager.depth);

                    // Spawn plans
                    for(int i = 0; i < iTargetDepth; i++)
                    {
                        Transform plans = Instantiate<Transform>(drillPlansPrefab);
                        plans.SetParent(transform);
                        plans.localPosition = new Vector3(0.05f, -1.03125f - i, 0.2f);

                        drillPlans.Add(plans);
                    }

                    Transform end = Instantiate<Transform>(drillPlansEndPrefab);
                    end.SetParent(transform);
                    end.localPosition = new Vector3(0.05f, -1.03125f - iTargetDepth, 0.2f);

                    drillPlans.Add(end);

                    drillBit = Instantiate<Transform>(drillBitEndPrefab);
                    drillBit.SetParent(transform);
                    drillBit.localPosition = new Vector3(0.05f, -0.625f, 0.1f);

                    drillPlans.Add(drillBit);

                    // Warn!
                    for (int i = 0; i < iTargetDepth; i++)
                    {
                        TileBase tb = Core.theTM.GetTileBase(iTargetX, i);
                        if(tb.IsLizardy())
                        {
                            tb.bWarning++;
                        }
                    }

                }
                break;
            case Phase.DRILL:

                // Do some drilling
                fDrillProgress += fDrillSpeed * Time.deltaTime;

                int iDrillProgress = Mathf.FloorToInt(fDrillProgress);
                float fRemainder = fDrillProgress - iDrillProgress;
                if (iDrillProgress >= drills.Count)
                {
                    drills.Add(Instantiate<SpriteRenderer>(drillBitPrefab));
                    topDrill = drills[drills.Count - 1];
                    topDrill.transform.SetParent(transform);

                    // We just hit a new spot.

                    if(iDrillProgress >= 1)
                    {
                        Core.theTM.HumanDigTile(iTargetX, iDrillProgress - 1);
                    }
                }

                for(int i = 0; i < drills.Count; i++)
                {
                    drills[i].transform.localPosition = new Vector3(0.05f, -0.03125f - i - fRemainder, 0.1f);
                }

                drillBit.localPosition = new Vector3(0.05f, -0.625f - fDrillProgress, 0.1f);

                break;
            case Phase.LEAVE:
                transform.localPosition += new Vector3(Time.deltaTime * fSpeed * (bFlip ? -1.0f : 1.0f), 0.0f, 0.0f);
                if(transform.position.x > 12.0f || transform.position.x < -12.0f)
                {
                    Destroy(gameObject);
                }
                break;
        }
		
	}

    private void CeaseDrilling()
    {
        SetAnim(walkAnim);
        phase = Phase.LEAVE;

        for (int i = 0; i < iTargetDepth; i++)
        {
            TileBase tb = Core.theTM.GetTileBase(iTargetX, i);
            if (tb.IsLizardy())
            {
                tb.bWarning--;
            }
        }

        for (int i = 0; i < drills.Count; i++)
        {
            Destroy(drills[i].gameObject);
        }

        drills.Clear();

        foreach(Transform t in drillPlans)
        {
            Destroy(t.gameObject);
        }

        drillPlans.Clear();
    }
}
