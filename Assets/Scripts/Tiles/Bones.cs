using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bones : TileBase
{
    public Resource bonesPrefab;

    public override TileBase.TileType Type()
    {
        return TileBase.TileType.BONES;
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

    public override void Destroy()
    {
        base.Destroy();
    }

    public override void Replace()
    {
        base.Replace();

        if (replacingTile == null)
            return;

        Resource metal = Instantiate<Resource>(bonesPrefab);
        metal.transform.position = transform.position + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), -1.0f);
        metal.PutInRoom(replacingTile);
        Core.theTM.RegisterResource(metal);
    }
}