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
        HUMAN_SKIN
    }

    public TileBase holder = null;
    public Lizard carriedBy = null;

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
        lizzo.carrying = this;
        carriedBy = lizzo;
    }

}
