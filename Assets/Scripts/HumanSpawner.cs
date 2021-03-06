﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSpawner : MonoBehaviour
{
    public static HumanSpawner INSTANCE;

    public Human[] humanPrefabs;
    public float fTimeToNextHuman = 0.0f;

    public DrillTruck drillTruckPrefab;
    public float fTimeToNextDrillTruck = 0.0f;
    public DrillTruck[] trucks = new DrillTruck[TileManager.width];

    public TunnelBore borePrefab;
    public float fTimeToNextBore = 0.0f;
    public TunnelBore[] bores = new TunnelBore[TileManager.depth];

	// Use this for initialization
	void Start () {
        INSTANCE = this;

    }

    public void BonesFound()
    {
        //fTimeToNextBore += 5.0f;
        //fTimeToNextDrillTruck += 5.0f;
    }

    // Update is called once per frame
    void Update ()
    {
        fTimeToNextHuman -= Time.deltaTime;
        if(fTimeToNextHuman <= 0.0f)
        {
            fTimeToNextHuman = Random.Range(0.5f, 1.0f);
            Human human = Instantiate<Human>(humanPrefabs[Random.Range(0, humanPrefabs.Length)]);
            bool bFlip = Random.value > 0.5f;
            human.bFlip = bFlip;
            human.transform.position = new Vector3(bFlip ? 8.0f : -8.0f, 0.375f, -3.0f);

        }

        fTimeToNextDrillTruck -= Time.deltaTime;
        if (fTimeToNextDrillTruck <= 0.0f)
        {
            fTimeToNextDrillTruck = Random.Range(20f, 30.0f);
            DrillTruck human = Instantiate<DrillTruck>(drillTruckPrefab);
            bool bFlip = Random.value > 0.5f;
            human.bFlip = bFlip;
            human.transform.position = new Vector3(bFlip ? 8.0f : -8.0f, 0.625f, -3.0f);
            human.iTargetX = Random.Range(0, TileManager.width);
            if (human.iTargetX == TileManager.width - 6)
                human.iTargetX = TileManager.width - 5;
            if(trucks[human.iTargetX] != null)
            {
                fTimeToNextDrillTruck /= 3.0f;
                Destroy(human.gameObject);
            }
            else
            {
                trucks[human.iTargetX] = human;
            } 
        }

        fTimeToNextBore -= Time.deltaTime;
        if (fTimeToNextBore <= 0.0f)
        {
            fTimeToNextBore = Random.Range(50.0f, 70.0f);
            TunnelBore human = Instantiate<TunnelBore>(borePrefab);
            bool bFlip = Random.value > 0.5f;
            human.bFlip = bFlip;
            human.iDepth = Random.Range(4, TileManager.depth);
			if (Random.Range(0, 10) < 2)
				human.iDepth -= 2;
            human.transform.position = new Vector3(bFlip ? 8.0f : -8.0f, -0.5f - human.iDepth, -3.0f);

            if (bores[human.iDepth] != null)
            {
                fTimeToNextBore /= 3.0f;
                Destroy(human.gameObject);
            }
            else
            {
                bores[human.iDepth] = human;
            }
        }
    }
}
