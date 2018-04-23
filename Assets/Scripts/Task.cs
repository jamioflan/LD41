using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Task
{ 
    public enum Type
    {
        SELL_RESOURCE,
        EAT,
        RELAX,
        WORK_ROOM,
        BUILD,
        TIDY,

		BREED,
		FARM,
		TRAP,
		TAILOR,
        GO_MAD,

	}
    public Lizard assignedLizard;
    public TileBase associatedTile;
    public Type type;
    public Dictionary<Resource.ResourceType, int> requiredResources; // The resources needed to carry out this task
    List<Resource> claimedResources; // The resources claimed by this task (these sit on the associated tile)
    Dictionary<Resource.ResourceType, int> missingResources; // The resources left over
    public void AddResource(Resource resource)
    {
        claimedResources.Add(resource);
        missingResources[resource.type] -= 1;
        resource.Drop();
    }
    public Resource.ResourceType GetNextMissing()
    {
        foreach (KeyValuePair<Resource.ResourceType, int> kv in missingResources)
            if (kv.Value > 0)
                return kv.Key;
        return Resource.ResourceType.NULL;
    }
    public Task(Type type_) : this(type_, new Dictionary<Resource.ResourceType, int>()) { }

    public Task (Type type_, Dictionary<Resource.ResourceType, int> required)
    {
        type = type_;
        requiredResources = required;
        claimedResources = new List<Resource>();
        missingResources = new Dictionary<Resource.ResourceType, int>();
        foreach (KeyValuePair<Resource.ResourceType, int> kv in required)
            missingResources.Add(kv.Key, kv.Value);
        if (type == Type.GO_MAD)
            associatedTile = Core.theTM.hutTile;
    }

    public void UseResources()
    {
        while (claimedResources.Count != 0)
        {
            claimedResources[0].Destroy();
            claimedResources.RemoveAt(0);
        }
    }

    public void Finish()
    {
		if (associatedTile != null)
		{
			associatedTile.SetTaskActive(false);
			associatedTile.queuedTask = null;
		}
        assignedLizard.currentTask = null;
    }
}
