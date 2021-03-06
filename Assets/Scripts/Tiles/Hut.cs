﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hut : TileBase {

    public override TileBase.TileType Type()
    {
        return TileBase.TileType.HUT;
    }

	public override bool CanBeBuiltOver() { return false; }
	public override bool CanBeDug() { return false; }
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
}
