using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class TileBase : MonoBehaviour {

    public int x;
    public int y;

    public enum TileType {
        EMPTY = 0,
        STORAGE = 1,
    }

    virtual public bool IsPassable()
    {
        return true;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void Destroy()
    {

    }


    public void SetCoords(int px, int py)
    {
        x = px;
        y = py;
        transform.position = new Vector3(x + 0.5f - TileManager.width / 2, -0.5f - y, -2);
    }

}
