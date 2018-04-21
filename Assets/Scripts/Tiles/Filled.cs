using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Filled : TileBase {
    
    override public TileBase.TileType Type()
    {
        return TileBase.TileType.FILLED;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    override public bool IsPassable()
    {
        return false;
    }
}
