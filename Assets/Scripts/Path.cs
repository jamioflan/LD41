using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Path {
    Stack<KeyValuePair<int, int>> _path = new Stack<KeyValuePair<int, int>>();

    public int endX;
    public int endY;

    public KeyValuePair<int, int> Pop()
    {
        return _path.Pop();
    }

    public void Push(KeyValuePair<int, int> point)
    {
        if (_path.Count == 0)
        {
            endX = point.Key;
            endY = point.Value;
        }
        _path.Push(point);
    }

    public int Count()
    {
        return _path.Count;
    }

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

    private static Point GetPoint(int x, int y, int currentCost, List<KeyValuePair<int, int>> targets)
    {
        var p = new Point();
        if (x < 0 || x >= TileManager.width || y < 0 || y >= TileManager.depth)
        {
            p.isAccessible = false;
            p.x = -1;
            p.y = -1;
            return p;
        }
        var t = Core.theTM.tiles[x, y];
        p.x = x;
        p.y = y;
        p.isAccessible = t.IsPassable();
        p.travelCost = currentCost + 1;
        p.manhattanCost = int.MaxValue;
        foreach (KeyValuePair<int, int> target in targets)
            p.manhattanCost = Mathf.Min(p.manhattanCost, Mathf.Abs(x - target.Key) + Mathf.Abs(y - target.Value));
        return p;
    }

    public static Path GetPath(KeyValuePair<int, int> from, TileBase to)
    {
        return GetPath(from, to.GetKVPair());
    }

    public static Path GetPath(KeyValuePair<int, int> from, KeyValuePair<int, int> to)
    {
        return GetPath(from, new List<KeyValuePair<int, int>>() { to });
    }

    public static Path GetPath(KeyValuePair<int, int> from, List<TileBase> to)
    {
        var kvList = new List<KeyValuePair<int, int>>();
        foreach (TileBase tile in to)
            kvList.Add(tile.GetKVPair());
        return GetPath(from, kvList);
    }

    public static Path GetPath(KeyValuePair<int, int> from, List<KeyValuePair<int, int>> to)
    {
        if (to.Count == 0)
            return null;
        Path path = new Path();
        if (to.Contains(from) )
        {
            path.Push(from);
            return path;
        }
        var considered = new HashSet<Point>(new PointHash());
        var open = new SortedDictionary<int, Point>();
        var closed = new SortedDictionary<int, Point>();

        Debug.Log("GetPath called for ");
        foreach (KeyValuePair<int, int> t in to)
            Debug.Log("  (" + t.Key + ", " + t.Value + ")");

        Point current = GetPoint(from.Key, from.Value, -1, to);
        KeyValuePair<int, int> chosenTarget = new KeyValuePair<int, int>();
        bool doLoop = true;
        

        while (doLoop)
        {

            // Get everything around the current point
            var next = new List<Point>() {
                GetPoint(current.x - 1, current.y, current.travelCost, to),
                GetPoint(current.x, current.y + 1, current.travelCost, to),
                GetPoint(current.x + 1, current.y, current.travelCost, to),
                GetPoint(current.x, current.y - 1, current.travelCost, to),
            };
            foreach (Point p in next)
            {
                Debug.Log("Consider Point: (" + p.x + ", " + p.y + "), travelCost: " + p.travelCost + ", manhattanCost: " + p.manhattanCost + ", isAccessible: "+p.isAccessible);
                if (!considered.Add(p))
                    // Returns false if it was already there
                    continue;

                if (to.Contains(new KeyValuePair<int, int>(p.x, p.y)) )
                {
                    // This means we've reached the target!
                    doLoop = false;
                    chosenTarget = new KeyValuePair<int, int>(p.x, p.y);
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
        path.Push(chosenTarget);
        path.Push(new KeyValuePair<int, int>(current.x, current.y));

        var itr = closed.GetEnumerator();
        while (current.travelCost > 0)
        {
            // The way we've set up the map it means that they are sorted by travel cost first
            // This means that we can advance the iterator until it's reached the set of points with the right travel cost
            int targetKey = -TileManager.width * TileManager.depth * (current.travelCost - 1);
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
