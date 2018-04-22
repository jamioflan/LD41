﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nest : TileBase {

    public override TileBase.TileType Type()
    {
        return TileBase.TileType.NEST;
    }

	public override bool CanBeBuiltOver() { return false; }
	public override bool CanBeDug() { return false; }
	public override bool CanBeFilledIn() { return true; }
	public override bool CanBeMarkedAsPriority() { return true; }

	public override void Start()
	{
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }
}
