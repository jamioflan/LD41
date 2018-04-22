using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Empty : TileBase {

    public override TileBase.TileType Type()
    {
        return TileBase.TileType.EMPTY;
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }
}
