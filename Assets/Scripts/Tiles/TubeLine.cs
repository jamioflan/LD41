using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeLine : TileBase {

    override public TileBase.TileType Type()
    {
        return TileBase.TileType.TUBE_LINE;
    }

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
}
