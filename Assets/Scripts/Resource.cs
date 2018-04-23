using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour

{
    public enum ResourceType
    {
        METAL,
        GEMS,
        MUSHROOMS,
        HUMAN_FOOD,
        HUMAN_SKIN,
        BONES, 
        NULL
    }

    public Lizard reservee = null;
    public ResourceType type;
    public TileBase holder = null;
    public Lizard carriedBy = null;

    public bool isClaimed = false;

	void Start () {
		
	}
	
	void Update () {
		
	}


    public void Drop()
    {
        if (holder != null)
        {
            holder.RemoveResource(this);
            holder = null;
        }
        if (carriedBy != null)
        {
            carriedBy.carrying = null;
            carriedBy = null;
        }
        
    }

    public void PutInRoom(TileBase room)
    {
        Drop();
        room.StoreResource(this);
        holder = room;
    }

    public void GiveToLizard(Lizard lizzo)
    {
        Drop();
        lizzo.Take(this);
        carriedBy = lizzo;
    }

    public void Unclaim()
    {
        if (!isClaimed)
            return;
        Core.theTM.AddToUnclaimed(this);
        isClaimed = false;
        reservee = null;
    }

    public void Claim(Lizard lizzo)
    {
        Core.theTM.RemoveFromUnclaimed(this);
        isClaimed = true;
        reservee = lizzo;
    }

    public void Destroy()
    {
        Core.theTM.allResources.Remove(this);
        if (!isClaimed)
            Core.theTM.RemoveFromUnclaimed(this);
        Destroy(gameObject);
    }

}
