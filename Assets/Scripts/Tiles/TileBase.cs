using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class TileBase : MonoBehaviour {

    public int x;
    public int y;

    public KeyValuePair<int, int> GetKVPair()
    {
        return new KeyValuePair<int, int>(x, y);
    }

    public SpriteRenderer warningSprite;
    private float fWarningTime = 0.0f;
    public int bWarning = 0; // Number of things intending to dig this tile
    public string printName = "";

	public SpriteRenderer highlightSprite;
	public bool bShouldBeHighlighted = false;

    static float fMaxBuildTime = 5.0f;
    float fBuildLeft = fMaxBuildTime;

    public bool isConstructed;

    public Resource[] tidyResources;
    public List<Resource> clutteredResources = new List<Resource>();
    public List<Lizard> lizardsOnTile = new List<Lizard>();

    public Transform[] tidyStorageSpots;

    public TileBase replacingTile = null;

    public Task queuedTask = null;

    [System.Serializable]
    public enum TileType {
        EMPTY = 0,
        FILLED = 1,
        HUT = 2,
        STORAGE = 3,
        HATCHERY = 4,
        NEST = 5,
        TAILOR = 6,
        TRAP = 7,
        FARM = 8,
        TVROOM = 9,

        METAL = 10,
        GEMS = 11,
        TUBE_LINE = 12,
        BONES = 13,
    }

    public void StoreResource(Resource resource)
    {
        resource.Drop();
        if (isConstructed)
            Core.theTM.AddToUnclaimed(resource);
        for (int i = 0; i < tidyStorageSpots.Length; i++)
        {
            if (tidyResources[i] == null)
            {
                tidyResources[i] = resource;
                resource.transform.SetParent(tidyStorageSpots[i]);
                resource.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                return;
            }
        }

        clutteredResources.Add(resource);
        resource.transform.SetParent(transform);
        resource.transform.localPosition = new Vector3(Random.Range(-0.285f, 0.285f), -0.0625f + Random.Range(0.0f, 0.05f), -1.0f);    
    }

    public int NEmptyResourceSlots()
    {
        int ret = 0;
        foreach (Resource res in tidyResources)
            if (res == null)
                ret += 1;
        return ret;
    }

    public void RemoveResource(Resource resource)
    {
        for (int i = 0; i < tidyStorageSpots.Length; i++)
        {
            if (tidyResources[i] == resource)
            {
                tidyResources[i] = null;
                return;
            }
        }

        clutteredResources.Remove(resource);
    }

    public Resource FindResource(Resource.ResourceType type)
    {
        foreach (Resource r in clutteredResources)
            if (r.type == type)
                return r;
        foreach (Resource r in tidyResources)
            if (r.type == type)
                return r;
        return null;
    }

    public void PurgeItems()
    {
        foreach (Resource r in clutteredResources)
            Destroy(r.gameObject);
        clutteredResources.Clear();
        foreach (Resource r in tidyResources)
            Destroy(r.gameObject);
        tidyResources = new Resource[tidyResources.Length];
    }

    public abstract TileType Type();

	public abstract bool CanBeBuiltOver();
	public abstract bool CanBeDug();
	public abstract bool CanBeFilledIn();
	public abstract bool CanBeMarkedAsPriority();

    virtual public bool IsPassable()
    {
        Debug.Log("IsPassable: " + isConstructed);
        return isConstructed;
    }



    public virtual void Replace()
    {
        if (replacingTile == null)
            return;
        GetComponentInParent<TileManager>().tiles[x, y] = replacingTile;
        replacingTile.SetCoords(x, y);
        replacingTile.transform.SetParent(transform.parent);

        // Stage on this list to avoid concurrent modification death!
        List<Resource> stuffToMove = new List<Resource>();
        foreach (Resource resource in clutteredResources)
            stuffToMove.Add(resource);

        foreach (Resource resource in tidyResources)
            stuffToMove.Add(resource);

        foreach(Resource resource in stuffToMove)
            replacingTile.StoreResource(resource);

        replacingTile.bWarning = bWarning;

        GetComponentInParent<TileManager>().UpdateEdges(x, y);

        while (lizardsOnTile.Count != 0)
            lizardsOnTile[0].SetTile(replacingTile);
        Destroy();
        if (queuedTask != null)
            queuedTask.associatedTile = replacingTile;
    }

    virtual protected void FinishConstruction()
    {
        if (isConstructed)
            return;
        isConstructed = true;
        foreach (Resource res in tidyResources)
            if (res != null)
                Core.theTM.AddToUnclaimed(res);
        foreach (Resource res in clutteredResources)
            if (res != null)
                Core.theTM.AddToUnclaimed(res);
    }

    // Return true if building is done!
    public bool Build(Lizard by)
    {
        if (replacingTile != null)
        {
            Replace();
            return false;
        }
        fBuildLeft -= Time.deltaTime;
        SetAlpha(Mathf.Min(1.0f, 0.3f + 0.7f * (1 - fBuildLeft / fMaxBuildTime)));

        if (fBuildLeft < 0)
            FinishConstruction();
        return isConstructed;
    }

    public void SetAlpha(float alpha)
    {
        foreach (SpriteRenderer renderer in GetComponents<SpriteRenderer>())
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
    }

    public bool IsLizardy()
    {
        switch(Type())
        {
            case TileType.FILLED:
            case TileType.GEMS:
            case TileType.METAL:
            case TileType.BONES:
                return false;
        }
        return true;
    }

    public virtual void Start ()
    {

        warningSprite.enabled = false;
		if (highlightSprite != null) { highlightSprite.enabled = false; }
		tidyResources = new Resource[tidyStorageSpots.Length];
    }
	
	public virtual void Update ()
    {
        warningSprite.enabled = IsLizardy() && bWarning > 0;

		if (highlightSprite != null)
		{
			highlightSprite.enabled = bShouldBeHighlighted;
		}

        if (bWarning > 0)
        {
            fWarningTime += Time.deltaTime * 2.0f;

            if(warningSprite != null)
            {
                float scale = 1.0f + 0.1f * Mathf.Sin(fWarningTime);
                warningSprite.transform.localScale = new Vector3(scale, scale, scale);
            }
        }
	}

    public virtual void Destroy()
    {
        while (lizardsOnTile.Count != 0)
            lizardsOnTile[0].SetTile(null);
		Destroy(gameObject);
    }


    public void SetCoords(int px, int py)
    {
        x = px;
        y = py;
        transform.position = new Vector3(x + 0.5f - TileManager.width / 2, -0.5f - y, -2);
    }

}
