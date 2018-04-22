using System.Collections;
using System.Collections.Generic;
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
        return Mathf.Abs(x - p.x) + Mathf.Abs(y - p.y);
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

    public SpriteRenderer connectionPrefabLR;
    public SpriteRenderer connectionPrefabUD;
    public SpriteRenderer[,] connectionsLR = new SpriteRenderer[width - 1, depth];
    public SpriteRenderer[,] connectionsUD = new SpriteRenderer[width, depth - 1];

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
                tiles[ii, jj] = Instantiate<TileBase>(prefabs[(int)TileBase.TileType.FILLED]);
                tiles[ii, jj].SetCoords(ii, jj);
                tiles[ii, jj].transform.SetParent(transform);
            }
        }

        for(int i = 0; i < width - 1; i++)
        {
            for(int j = 0; j < depth; j++)
            {
                connectionsLR[i, j] = Instantiate<SpriteRenderer>(connectionPrefabLR);
                connectionsLR[i, j].transform.SetParent(transform);
                connectionsLR[i, j].transform.position = new Vector3(i + 1.0f - TileManager.width / 2, -0.5f - j, -2.1f);
                connectionsLR[i, j].enabled = false;
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth - 1; j++)
            {
                connectionsUD[i, j] = Instantiate<SpriteRenderer>(connectionPrefabUD);
                connectionsUD[i, j].transform.SetParent(transform);
                connectionsUD[i, j].transform.position = new Vector3(i + 0.5f - TileManager.width / 2, -j - 1.0f, -2.1f);
                connectionsUD[i, j].enabled = false;
            }
        }

        Debug.Log("Create starting city");


        hutTile = RequestNewTile(width -2 , 0, TileBase.TileType.HUT, true) as Hut;
        RequestNewTile(width - 2, 1, TileBase.TileType.STORAGE, true);
        RequestNewTile(width - 3, 1, TileBase.TileType.NEST, true);

        for (int i = 0; i < 3; i++)
        {
            int x = Random.Range(0, width), y = Random.Range(0, 7);
            if (tiles[x, y].Type() == TileBase.TileType.FILLED)
                RequestNewTile(x, y, TileBase.TileType.BONES, true);
        }

        for (int i = 0; i < 10; i++)
        {
            int x = Random.Range(0, width), y = Random.Range(0, 7);
            if (tiles[x, y].Type() == TileBase.TileType.FILLED)
                RequestNewTile(x, y, TileBase.TileType.METAL, true);
        }
        for (int i = 0; i < 2; i++)
        {
            int x = Random.Range(0, width), y = Random.Range(0, 7);
            if(tiles[x,y].Type() == TileBase.TileType.FILLED)
                RequestNewTile(x, y, TileBase.TileType.GEMS, true);
        }

        for (int i = 0; i < 20; i++)
        {
            int x = Random.Range(0, width), y = Random.Range(8, depth);
            if (tiles[x, y].Type() == TileBase.TileType.FILLED)
                RequestNewTile(x, y, TileBase.TileType.METAL, true);
        }
        for (int i = 0; i < 8; i++)
        {
            int x = Random.Range(0, width), y = Random.Range(8, depth);
            if (tiles[x, y].Type() == TileBase.TileType.FILLED)
                RequestNewTile(x, y, TileBase.TileType.GEMS, true);
        }
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

    // Return whether to continue digging
    public bool HumanDigTile(int x, int y)
    {
        if (x < 0 || y < 0 || x >= width || y >= depth)
        {
            return false;
        }

        switch(tiles[x,y].Type())
        {
            case TileBase.TileType.METAL:
            case TileBase.TileType.GEMS:
            case TileBase.TileType.BONES:
                RequestNewTile(x, y, TileBase.TileType.FILLED, true);
                return false;

            case TileBase.TileType.FILLED:
                return true;

            case TileBase.TileType.TUBE_LINE:
                return false;

            case TileBase.TileType.EMPTY:
                Player.thePlayer.AddSuspicion(10.0f);
                return false;

            case TileBase.TileType.HUT:
            case TileBase.TileType.STORAGE:
            case TileBase.TileType.NEST:
            case TileBase.TileType.FARM:
            case TileBase.TileType.TVROOM:
            case TileBase.TileType.TRAP:
                RequestNewTile(x, y, TileBase.TileType.EMPTY, true);
                Player.thePlayer.AddSuspicion(10.0f);
                return false;

            case TileBase.TileType.HATCHERY:
                RequestNewTile(x, y, TileBase.TileType.EMPTY, true);
                Player.thePlayer.AddSuspicion(25.0f);
                return false;

            case TileBase.TileType.TAILOR:
                RequestNewTile(x, y, TileBase.TileType.EMPTY, true);
                Player.thePlayer.AddSuspicion(50.0f);
                return false;           

        }


        return true;
    }

    public TileBase GetTileBase(int x, int y)
    {
        return tiles[x, y];
    }

    public void UpdateEdges(int x, int y)
    {
        if(tiles[x,y].IsLizardy())
        {
            if(x > 0)
                connectionsLR[x - 1, y].enabled = tiles[x - 1, y].IsLizardy();
            if(x < width - 1)
                connectionsLR[x, y].enabled = tiles[x + 1, y].IsLizardy();
            if (y > 0)
                connectionsUD[x, y - 1].enabled = tiles[x, y - 1].IsLizardy();
            if (y < depth - 1)
                connectionsUD[x, y].enabled = tiles[x, y + 1].IsLizardy();
        }
        else
        {
            if(x > 0)
                connectionsLR[x - 1, y].enabled = false;
            if(x < width - 1)
                connectionsLR[x, y].enabled = false;
            if(y > 0)
                connectionsUD[x, y - 1].enabled = false;
            if(y < depth - 1)
                connectionsUD[x, y].enabled = false;
        }
    }

    // Create a new tile
    public TileBase RequestNewTile(int x, int y, TileBase.TileType type, bool instant = false)
    {
        TileBase newTile = Instantiate<TileBase>(prefabs[(int)type]);
        newTile.SetCoords(-100, -100);
        tiles[x, y].replacingTile = newTile;
        
        if (instant)
        {
            tiles[x, y].Replace();
            tiles[x, y].fBuildLeft = -Time.deltaTime;
        }
        else
        {
            Task task = new Task(Task.Type.BUILD);
            task.targetX = x;
            task.targetY = y;

            var tl = new Queue<Task>();
            tl.Enqueue(task);
            Player.thePlayer.pendingWorkerTasks.Add(tl);

        }

        return tiles[x, y];
    }

    private Point GetPoint(int x, int y, int currentCost, int targetX, int targetY)
    {
        var p = new Point();
        if (x < 0 || x >= TileManager.width || y < 0 || y >= TileManager.depth)
        {
            p.isAccessible = false;
            p.x = -1;
            p.y = -1;
            return p;
        }
        var t = tiles[x, y];    
        p.x = x;
        p.y = y;
        p.isAccessible = t.IsPassable();
        p.travelCost = currentCost + 1;
        p.manhattanCost = Mathf.Abs(x - targetX) + Mathf.Abs(y - targetY);
        return p;
    }



    public Stack<KeyValuePair<int, int>> GetPath(KeyValuePair<int, int> from, KeyValuePair<int, int> to)
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

                if (p.x == targetX && p.y == targetY)
                {
                    // This means we've reached the target!
                    doLoop = false;
                    break;
                }
                if (!p.isAccessible)
                    continue;
                open.Add(p.OpenKey(), p);
            }
            if (!doLoop)
                break;
            // Now look at the first 'open' point
            if (open.Count == 0)
            {
                Debug.Log("Failed to build path - open set is empty!");
                return null;
            }
            var keyItr = open.Keys.GetEnumerator();
            keyItr.MoveNext();
            var nextKey = keyItr.Current;
            // Add 'current' to closed, remove this one from 'open' and set it to be 'current'
            closed.Add(current.ClosedKey(), current);
            current = open[nextKey];
            open.Remove(nextKey);
        }

        // Now we can build the path
        var path = new Stack<KeyValuePair<int, int>>();
        path.Push(to);
        path.Push(new KeyValuePair<int, int>(current.x, current.y));

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
            path.Push(new KeyValuePair<int, int>(current.x, current.y));
        }


        return path;
    }


}
