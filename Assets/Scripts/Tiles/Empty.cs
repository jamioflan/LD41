﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Empty : TileBase {
    
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