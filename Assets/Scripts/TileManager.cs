using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class TileManager : MonoBehaviour {

    public static int width = 10;
    public static int depth = 15;

    public TileBase[] prefabs;

    public TileBase[,] tiles = new TileBase[width, depth];
    public Hut hutTile;

    public Lizard lizardPrefab;
    public Dictionary<Lizard.Assignment, List<Lizard>> lizards = new Dictionary<Lizard.Assignment, List<Lizard>>();
	public int[] maxAssigned = new int[(int)Lizard.Assignment.NUM_ASSIGNMENTS] { 0, 0, 0, 0, 0 };

	public Resource foodPrefab;

	public SpriteRenderer connectionPrefabLR;
    public SpriteRenderer connectionPrefabUD;
    public SpriteRenderer[,] connectionsLR = new SpriteRenderer[width - 1, depth];
    public SpriteRenderer[,] connectionsUD = new SpriteRenderer[width, depth - 1];

    public List<Resource> allResources = new List<Resource>();
    public Dictionary<Resource.ResourceType, List<Resource>> unclaimedResources = new Dictionary<Resource.ResourceType, List<Resource>>();

	public int GetMaxNumBreeders() { return maxAssigned[(int)Lizard.Assignment.HATCHERY]; }
	public int GetMaxNumFarmers() { return maxAssigned[(int)Lizard.Assignment.FARMER]; }
	public int GetMaxNumTrappers() { return maxAssigned[(int)Lizard.Assignment.TRAP]; }
	public int GetMaxNumTailors() { return maxAssigned[(int)Lizard.Assignment.TAILOR]; }

	public int GetNumBreeders() { return lizards[Lizard.Assignment.HATCHERY].Count; }
	public int GetNumFarmers() { return lizards[Lizard.Assignment.FARMER].Count; }
	public int GetNumTrappers() { return lizards[Lizard.Assignment.TRAP].Count; }
	public int GetNumTailors() { return lizards[Lizard.Assignment.TAILOR].Count; }
	public int GetNumMisc() { return lizards[Lizard.Assignment.WORKER].Count; }

	public void AddAssignmnets(Lizard.Assignment ass, int num)
	{
		maxAssigned[(int)ass] += num;
	}

	// Use this for initialization
	void Start () {
        // Set up lizard dict
        lizards.Add(Lizard.Assignment.HATCHERY, new List<Lizard>());
        lizards.Add(Lizard.Assignment.TRAP, new List<Lizard>());
        lizards.Add(Lizard.Assignment.TAILOR, new List<Lizard>());
        lizards.Add(Lizard.Assignment.WORKER, new List<Lizard>());
		lizards.Add(Lizard.Assignment.FARMER, new List<Lizard>());

		// Set up resource dict
		unclaimedResources.Add(Resource.ResourceType.METAL, new List<Resource>());
        unclaimedResources.Add(Resource.ResourceType.GEMS, new List<Resource>());
        unclaimedResources.Add(Resource.ResourceType.MUSHROOMS, new List<Resource>());
        unclaimedResources.Add(Resource.ResourceType.HUMAN_FOOD, new List<Resource>());
        unclaimedResources.Add(Resource.ResourceType.HUMAN_SKIN, new List<Resource>());
        unclaimedResources.Add(Resource.ResourceType.BONES, new List<Resource>());

        for (int ii = 0; ii < width; ++ii)
        {
            for (int jj = 0; jj < depth; ++jj)
            {
                tiles[ii, jj] = Instantiate<TileBase>(prefabs[(int)TileBase.TileType.FILLED]);
				tiles[ii, jj].isConstructed = true;
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


        hutTile = RequestNewTile(width - 6 , 0, TileBase.TileType.HUT, true) as Hut;
		for(int i = 0; i < 6; i++)
			Instantiate < Resource > (foodPrefab).PutInRoom(hutTile);
        RequestNewTile(width - 6, 1, TileBase.TileType.STORAGE, true);
        RequestNewTile(width - 5, 1, TileBase.TileType.NEST, true);

        for (int i = 0; i < 6; i++)
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

        for (int i = 0; i < 40; i++)
        {
            int x = Random.Range(0, width), y = Random.Range(8, depth);
            if (tiles[x, y].Type() == TileBase.TileType.FILLED)
                RequestNewTile(x, y, TileBase.TileType.METAL, true);
        }
        for (int i = 0; i < 16; i++)
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
        lizard.SetTile(tile);
        lizard.assignment = Lizard.Assignment.WORKER;
        lizards[Lizard.Assignment.WORKER].Add(lizard);
        return lizard;
    }


    public void Reset()
	{
        foreach (List<Lizard> llist in lizards.Values)
            while (llist.Count != 0)
                llist[0].Destroy();

        CreateLizard(hutTile.x, hutTile.y).lizardName = "Squidge";
		CreateLizard(hutTile.x, hutTile.y).lizardName = "Nina";
		CreateLizard(hutTile.x, hutTile.y).lizardName = "Frank";
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
                RequestNewTile(x, y, TileBase.TileType.FILLED, true);
                tiles[x, y].PurgeItems();
                TextTicker.AddLine("The humans found resources and left");
                return false;

            case TileBase.TileType.BONES:
                //RequestNewTile(x, y, TileBase.TileType.FILLED, true);
                HumanSpawner.INSTANCE.BonesFound();
                //tiles[x, y].PurgeItems();
                TextTicker.AddLine("The humans found some dinosaur bones");
                TextTicker.AddLine("They will stop digging for now");
                return false;

            case TileBase.TileType.FILLED:
                return true;

            case TileBase.TileType.TUBE_LINE:
                return false;

            case TileBase.TileType.EMPTY:
				RequestNewTile(x, y, TileBase.TileType.FILLED, true);
				Player.thePlayer.AddSuspicion(10.0f);
                TextTicker.AddLine("The humans discovered a lizard hole");
                TextTicker.AddLine("Suspicion levels have increased");
                return false;

            case TileBase.TileType.HUT:
            case TileBase.TileType.STORAGE:
            case TileBase.TileType.NEST:
            case TileBase.TileType.FARM:
            case TileBase.TileType.TVROOM:
            case TileBase.TileType.TRAP:
                RequestNewTile(x, y, TileBase.TileType.FILLED, true);
                Player.thePlayer.AddSuspicion(10.0f);
                TextTicker.AddLine("A human discovered lizard habitation underground");
                TextTicker.AddLine("No one really believed them, so the impact was minimal");
                return false;

            case TileBase.TileType.HATCHERY:
                RequestNewTile(x, y, TileBase.TileType.FILLED, true);
                Player.thePlayer.AddSuspicion(25.0f);
                TextTicker.AddLine("The humans found a hatchery!");
                TextTicker.AddLine("Humantown residents demand to know more");
                return false;

            case TileBase.TileType.TAILOR:
                RequestNewTile(x, y, TileBase.TileType.FILLED, true);
                Player.thePlayer.AddSuspicion(50.0f);
                TextTicker.AddLine("The humans discovered a tailor!");
                TextTicker.AddLine("The Humantown Herald is on our case!");
                return false;           

        }


        return true;
    }

    public TileBase GetTileBase(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= depth)
            return null;
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
    public TileBase RequestNewTile(int x, int y, TileBase.TileType type, bool instant = false, int iMetalCost = 0, Resource.ResourceType resource = Resource.ResourceType.METAL)
    {
        TileBase newTile = Instantiate<TileBase>(prefabs[(int)type]);
        newTile.SetCoords(-100, -100);
        tiles[x, y].replacingTile = newTile;
        
        if (instant)
        {
            tiles[x, y].Replace();
            tiles[x, y].isConstructed = true;
        }
        else
        {
            Task task = new Task(Task.Type.BUILD, new Dictionary<Resource.ResourceType, int>() { { resource, iMetalCost } });
            task.associatedTile = tiles[x, y];
            newTile.isConstructed = false;
            Player.thePlayer.pendingTasks[(int)Lizard.Assignment.WORKER].Add(task);
        }

        return newTile;
    }

    public void RegisterResource(Resource resource)
    {
        allResources.Add(resource);
    }

    public bool AddToUnclaimed(Resource resource)
    {
        if (unclaimedResources[resource.type].Contains(resource))
        {
            Debug.Log("Trying to add unclaimed resource!");
            return false;
        }
        unclaimedResources[resource.type].Add(resource);
        Player.thePlayer.IncrementResourceCount(resource.type);
        return true;
    }

    public bool RemoveFromUnclaimed(Resource resource)
    {
        if (!unclaimedResources[resource.type].Contains(resource))
        {
            Debug.Log("Trying to remove claimed resource!");
            return false;
        }
        unclaimedResources[resource.type].Remove(resource);
        Player.thePlayer.IncrementResourceCount(resource.type, -1);
        return true;
    }

    public List<TileBase> GetTilesContaining(Resource.ResourceType type)
    {
        var ret = new List<TileBase>();
        if (type == Resource.ResourceType.NULL)
        {
            foreach (List<Resource> resList in unclaimedResources.Values)
                foreach (Resource res in resList)
                    if (res.holder != null)
                        if (!ret.Contains(res.holder))
                            ret.Add(res.holder);
        }
        else
            foreach (Resource res in unclaimedResources[type])
                if (res.holder != null)
                    if (!ret.Contains(res.holder))
                        ret.Add(res.holder);
        return ret;
    }



    public delegate bool TestTile(TileBase tile);

    public List<TileBase> GetTiles(TestTile del)
    {
        var ret = new List<TileBase>();
        foreach (TileBase tile in tiles)
            if (del(tile))
                ret.Add(tile);
        return ret;
    }

    public List<TileBase> GetClutteredTiles()
    {
        TestTile del = delegate (TileBase tile)
        {
            if (!tile.isConstructed)
                return false;
            foreach (Resource res in tile.clutteredResources)
                if (!res.isClaimed)
                    return true;
            return false;
        };
        return GetTiles(del);
    }

	public int GetNumTilesOfType( TileBase.TileType eType )
	{
		TileManager.TestTile del = delegate (TileBase tile)
		{
			return tile.Type() == eType;
		};
		return GetTiles(del).Count;
	}

	public void IncBreeders()
	{
		if (GetNumBreeders() < GetMaxNumBreeders() && GetNumMisc() >= 1)
		{
			Lizard liz = lizards[Lizard.Assignment.WORKER][0];
			lizards[Lizard.Assignment.WORKER].RemoveAt(0);
			liz.assignment = Lizard.Assignment.HATCHERY;
			lizards[Lizard.Assignment.HATCHERY].Add(liz);
		}
	}

	public void IncFarmers()
	{
		if (GetNumFarmers() < GetMaxNumFarmers() && GetNumMisc() >= 1)
		{
			Lizard liz = lizards[Lizard.Assignment.WORKER][0];
			lizards[Lizard.Assignment.WORKER].RemoveAt(0);
			liz.assignment = Lizard.Assignment.FARMER;
			lizards[Lizard.Assignment.FARMER].Add(liz);
		}
	}

	public void IncTrappers()
	{
		if (GetNumTrappers() < GetMaxNumTrappers() && GetNumMisc() >= 1)
		{
			Lizard liz = lizards[Lizard.Assignment.WORKER][0];
			lizards[Lizard.Assignment.WORKER].RemoveAt(0);
			liz.assignment = Lizard.Assignment.TRAP;
			lizards[Lizard.Assignment.TRAP].Add(liz);
		}
	}

	public void IncTailors()
	{
		if (GetNumTailors() < GetMaxNumTailors() && GetNumMisc() >= 1)
		{
			Lizard liz = lizards[Lizard.Assignment.WORKER][0];
			lizards[Lizard.Assignment.WORKER].RemoveAt(0);
			liz.assignment = Lizard.Assignment.TAILOR;
			lizards[Lizard.Assignment.TAILOR].Add(liz);
		}
	}

	public void DecBreeders()
	{
		if (GetNumBreeders() >= 1)
		{
			Lizard liz = lizards[Lizard.Assignment.HATCHERY][0];
			lizards[Lizard.Assignment.HATCHERY].RemoveAt(0);
			liz.assignment = Lizard.Assignment.WORKER;
			lizards[Lizard.Assignment.WORKER].Add(liz);
		}
	}
	public void DecFarmers()
	{
		if (GetNumFarmers() >= 1)
		{
			Lizard liz = lizards[Lizard.Assignment.FARMER][0];
			lizards[Lizard.Assignment.FARMER].RemoveAt(0);
			liz.assignment = Lizard.Assignment.WORKER;
			lizards[Lizard.Assignment.WORKER].Add(liz);
		}
	}
	public void DecTrappers()
	{
		if (GetNumTrappers() >= 1)
		{
			Lizard liz = lizards[Lizard.Assignment.TRAP][0];
			lizards[Lizard.Assignment.TRAP].RemoveAt(0);
			liz.assignment = Lizard.Assignment.WORKER;
			lizards[Lizard.Assignment.WORKER].Add(liz);
		}
	}
	public void DecTailors()
	{
		if (GetNumTailors() >= 1)
		{
			Lizard liz = lizards[Lizard.Assignment.TAILOR][0];
			lizards[Lizard.Assignment.TAILOR].RemoveAt(0);
			liz.assignment = Lizard.Assignment.WORKER;
			lizards[Lizard.Assignment.WORKER].Add(liz);
		}
	}
}
