using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour {

    public static int width = 10;
    public static int depth = 50;

    public TileBase[] prefabs;

    public TileBase[,] tiles = new TileBase[width, depth];

	// Use this for initialization
	void Start () {
        for (int ii = 0; ii < width; ++ii)
        {
            for (int jj = 0; jj < depth; ++jj)
            {
                if (jj == 0 && (ii == 5 || ii == 6))
                {
                    CreateNewTile(ii, jj, TileBase.TileType.STORAGE);
                }
                else
                {
                    CreateNewTile(ii, jj, TileBase.TileType.EMPTY);
                }
            }
        }
        		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Create a new tile
    TileBase CreateNewTile(int x, int y, TileBase.TileType type)
    {
        if (tiles[x, y] != null)
        {
            tiles[x, y].Destroy();
        }
        tiles[x, y] = Instantiate<TileBase>(prefabs[(int)type]);
        tiles[x, y].transform.SetParent(transform);
        tiles[x, y].SetCoords(x, y);
        return tiles[x, y];
    }


}
