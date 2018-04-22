using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

struct Point
{
    public int x;
    public int y;
    public bool isAccessible;
    public int travelCost;
    public int manhattanCost;
    public static int CoordinateCost(int x, int y)
    {
        return TileManager.width * y + x;
    }
    // For the open set first sort by the total estimated cost, then by a unique number set by the coordinates
    public int OpenKey()
    {
        return
            TileManager.width * TileManager.depth * (travelCost + manhattanCost) +
            Point.CoordinateCost(x, y);
    }
    // For the closed set first reverse sort by the travel cost then by a unique number set by the coordinates
    public int ClosedKey()
    {
        return
            -TileManager.width * TileManager.depth * travelCost +
            Point.CoordinateCost(x, y);
    }
    public int DistanceTo(Point p)
    {
        return Math.Abs(x - p.x) + Math.Abs(y - p.y);
    }

}

struct PointHash : IEqualityComparer<Point>
{
    public bool Equals(Point p1, Point p2)
    {
        return p1.x == p2.x && p1.y == p2.y;
    }

    public int GetHashCode(Point p1)
    {
        return Point.CoordinateCost(p1.x, p1.y).GetHashCode();
    }
}


public class TileManager : MonoBehaviour {

    public static int width = 10;
    public static int depth = 15;

    public TileBase[] prefabs;

    public TileBase[,] tiles = new TileBase[width, depth];
    public Hut hutTile;

    public Lizard lizardPrefab;
    public Dictionary<Lizard.Assignment, List<Lizard>> lizards = new Dictionary<Lizard.Assignment, List<Lizard>>();


	// Use this for initialization
	void Start () {
        // Set up lizard dict
        lizards.Add(Lizard.Assignment.HATCHERY, new List<Lizard>());
        lizards.Add(Lizard.Assignment.TRAP, new List<Lizard>());
        lizards.Add(Lizard.Assignment.TAILOR, new List<Lizard>());
        lizards.Add(Lizard.Assignment.WORKER, new List<Lizard>());

        for (int ii = 0; ii < width; ++ii)
        {
            for (int jj = 0; jj < depth; ++jj)
            {
                TileBase.TileType type = TileBase.TileType.FILLED;
                if (jj == 0)
                {
                    if (ii == width - 2)
                        type = TileBase.TileType.HUT;
                    else if (ii == width - 3)
                        type = TileBase.TileType.STORAGE;
                    else if (ii == width - 4)
                        type = TileBase.TileType.NEST;
                        
                }

                RequestNewTile(ii, jj, type, true);

            }
        }

        hutTile = tiles[width - 2, 0] as Hut;
        		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public Lizard CreateLizard(int x, int y)
    {
        var lizard = Instantiate<Lizard>(lizardPrefab);
        TileBase tile = tiles[x, y];
        lizard.transform.position = Lizard.GetTileCenter(tile);
        lizard.currentTile = tile;
        lizards[Lizard.Assignment.WORKER].Add(lizard);
        return lizard;
    }


    public void Reset()
	{
        foreach (List<Lizard> llist in lizards.Values)
            while (llist.Count != 0)
                llist[0].Destroy();
        CreateLizard(hutTile.x, hutTile.y);

    }

    public void HumanDigTile(int x, int y)
    {

    }

    public TileBase GetTileBase(int x, int y)
    {
        return tiles[x, y];
    }


    // Create a new tile
    public TileBase RequestNewTile(int x, int y, TileBase.TileType type, bool instant = false)
    {
        if (tiles[x, y] != null)
        {
            tiles[x, y].Destroy();
        }
        tiles[x, y] = Instantiate<TileBase>(prefabs[(int)type]);
        tiles[x, y].transform.SetParent(transform);
        tiles[x, y].SetCoords(x, y);
        if (instant)
        {
            tiles[x, y].fBuildLeft = -Time.deltaTime;
        }
        return tiles[x, y];
    }

    private Point GetPoint(int x, int y, int currentCost, int targetX, int targetY)
    {
        var p = new Point();
        if (x < 0 || x >= TileManager.width || y < 0 || y >= TileManager.depth)
        {
            p.isAccessible = false;
            return p;
        }
        var t = tiles[x, y];    
        p.x = x;
        p.y = y;
        p.isAccessible = t.IsPassable();
        p.travelCost = currentCost + 1;
        p.manhattanCost = Math.Abs(x - targetX) + Math.Abs(y - targetY);
        return p;
    }



    public List<KeyValuePair<int, int>> GetPath(KeyValuePair<int, int> from, KeyValuePair<int, int> to)
    {
        var considered = new HashSet<Point>(new PointHash());
        var open = new SortedDictionary<int, Point>();
        var closed = new SortedDictionary<int, Point>();

        int targetX = to.Key;
        int targetY = to.Value;
        Point current = GetPoint(from.Key, from.Value, -1, targetX, targetY);
        bool doLoop = true;
        while (doLoop)
        {
            
            // Get everything around the current point
            var next = new List<Point>() {
                GetPoint(current.x - 1, current.y, current.travelCost, targetX, targetY),
                GetPoint(current.x, current.y + 1, current.travelCost, targetX, targetY),
                GetPoint(current.x + 1, current.y, current.travelCost, targetX, targetY),
                GetPoint(current.x, current.y - 1, current.travelCost, targetX, targetY),
            };
            foreach (Point p in next)
            {
                if (!considered.Add(p))
                    // Returns false if it was already there
                    continue;
                if (p.isAccessible)
                    continue;
                if (p.x == targetX && p.y == targetY)
                {
                    // This means we've reached the target!
                    doLoop = false;
                    break;
                }
                open.Add(p.OpenKey(), p);
            }
            if (!doLoop)
                break;
            // Now look at the first 'open' point
            if (open.Count == 0)
                return null;
            var nextKey = open.Keys.GetEnumerator().Current;
            // Add 'current' to closed, remove this one from 'open' and set it to be 'current'
            closed.Add(current.ClosedKey(), current);
            current = open[nextKey];
            open.Remove(nextKey);
        }

        // Now we can build the path
        var path = new List<KeyValuePair<int, int>>()
        {
            to,
            new KeyValuePair<int, int>(current.x, current.y)
        };
        var itr = closed.GetEnumerator();
        while(current.travelCost > 0)
        {
            // The way we've set up the map it means that they are sorted by travel cost first
            // This means that we can advance the iterator until it's reached the set of points with the right travel cost
            int targetKey = - TileManager.width * TileManager.depth * (current.travelCost - 1);
            while (itr.Current.Key < targetKey)
                itr.MoveNext();
            // Now advance until we are next to the 'current' one
            while (itr.Current.Value.DistanceTo(current) > 1)
                itr.MoveNext();
            // We must be next to a new point! Add it to the path and then go again
            current = itr.Current.Value;
            path.Add(new KeyValuePair<int, int>(current.x, current.y));
        }

        return path;
    }


}
