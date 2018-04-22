using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSpawner : MonoBehaviour
{
    public static HumanSpawner INSTANCE;

    public Human[] humanPrefabs;
    public float fTimeToNextHuman = 0.0f;

    public DrillTruck drillTruckPrefab;
    public float fTimeToNextDrillTruck = 0.0f;

    public TunnelBore borePrefab;
    public float fTimeToNextBore = 0.0f;

	// Use this for initialization
	void Start () {
        INSTANCE = this;

    }

    public void BonesFound()
    {
        fTimeToNextBore += 10.0f;
        fTimeToNextDrillTruck += 20.0f;
    }

    // Update is called once per frame
    void Update () {
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
            if (human.iTargetX == TileManager.width - 2)
                human.iTargetX = TileManager.width / 2;
        }

        fTimeToNextBore -= Time.deltaTime;

        if (fTimeToNextBore <= 0.0f)
        {
            fTimeToNextBore = Random.Range(10f, 20.0f);
            TunnelBore human = Instantiate<TunnelBore>(borePrefab);
            bool bFlip = Random.value > 0.5f;
            human.bFlip = bFlip;
            human.iDepth = Random.Range(3, TileManager.depth);
            human.transform.position = new Vector3(bFlip ? 8.0f : -8.0f, -0.5f - human.iDepth, -3.0f);
        }
    }
}
