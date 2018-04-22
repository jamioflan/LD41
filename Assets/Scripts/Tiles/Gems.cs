using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gems : TileBase
{
    public Resource gemPrefab;

    public override TileBase.TileType Type()
    {
        return TileBase.TileType.GEMS;
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Destroy()
    {
        base.Destroy();

        int num = Random.Range(0, 4);
        for (int i = 0; i < num; i++)
        {
            Resource gem = Instantiate<Resource>(gemPrefab);
            gem.transform.position = transform.position + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), -1.0f);
        }

    }
}
