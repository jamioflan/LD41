using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metal : TileBase
{
    public Resource metalPrefab;

    public override TileBase.TileType Type()
    {
        return TileBase.TileType.METAL;
    }

	public override bool CanBeBuiltOver() { return false; }
	public override bool CanBeDug() { return true; }
	public override bool CanBeFilledIn() { return false; }
	public override bool CanBeMarkedAsPriority() { return true; }

	public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

	override public bool IsPassable()
	{
		return false;
	}

	public override void Destroy()
    {
        base.Destroy();



    }

    public override void Replace()
    {
        base.Replace();

        if (replacingTile == null)
            return;

        int num = Random.Range(0, 4);
        for (int i = 0; i < num; i++)
        {
            Resource metal = Instantiate<Resource>(metalPrefab);
            metal.transform.position = transform.position + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), -1.0f);
            replacingTile.StoreResource(metal);
            Core.theTM.RegisterResource(metal);
        }
    }
}
