using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Empty : TileBase {

    public override TileBase.TileType Type()
    {
        return TileBase.TileType.EMPTY;
    }

	public override bool CanBeBuiltOver() { return true; }
	public override bool CanBeDug() { return false; }
	public override bool CanBeFilledIn() { return true; }
	public override bool CanBeMarkedAsPriority() { return false; }

	public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }
}
