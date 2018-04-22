using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeLine : TileBase {

    override public TileBase.TileType Type()
    {
        return TileBase.TileType.TUBE_LINE;
    }

    public override bool CanBeBuiltOver() { return false; }
    public override bool CanBeDug() { return false; }
    public override bool CanBeFilledIn() { return false; }
    public override bool CanBeMarkedAsPriority() { return false; }

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
