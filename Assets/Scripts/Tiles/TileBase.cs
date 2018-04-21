using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class TileBase : MonoBehaviour {

    public int x;
    public int y;

    public enum TileType {
        EMPTY = 0,
        FILLED = 1,
        HUT = 2,
        STORAGE = 3,
        HATCHERY = 4,
        NEST = 5,
        TAILOR = 6
    }

    public abstract TileType Type();

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
		Destroy(gameObject);
    }


    public void SetCoords(int px, int py)
    {
        x = px;
        y = py;
        transform.position = new Vector3(x + 0.5f - TileManager.width / 2, -0.5f - y, -2);
    }

}
