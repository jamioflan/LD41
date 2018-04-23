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

		transform.SetParent(null);
    }

    public void PutInRoom(TileBase room)
    {
		// Drop it
        Drop();

		// Unclaim it
		if(isClaimed)
			Unclaim();

		// Now we are clear to store it
        room.StoreResource(this);
        holder = room;
    }

    public void GiveToLizard(Lizard lizzo)
    {
		// Drop it
		Drop();

		// Drop lizzo's existing item
		if (lizzo.carrying != null)
			lizzo.carrying.Drop();

		// Make sure lizzo's only claim is us
		if (lizzo.claimed != this)
			Claim(lizzo);

		// Carry
		lizzo.carrying = this;
		transform.SetParent(lizzo.transform);
		transform.localPosition = new Vector3(0.0f, 0.0f, -0.1f);
		carriedBy = lizzo;
    }

    public void Unclaim()
    {
        if (!isClaimed)
            return;
		Debug.Assert(reservee != null, "This is messed up as heck");

		// Can't revoke claim if we are still carrying it
		if(carriedBy == reservee)
		{
			Debug.Assert(false, "Lizard is trying to revoke claim on item it is holding");
			Drop();
		}

		// Break linkage
        Core.theTM.AddToUnclaimed(this);
		reservee.claimed = null;
		reservee = null;
		isClaimed = false;
	}

    public void Claim(Lizard lizzo)
    {
		// Check whether we are already claimed
		if(isClaimed)
		{
			Debug.Assert(false, "Bit cheeky. Stealing from another lizzo");
			Unclaim();
		}

		// Check whether the lizzo has prior commitments
		if(lizzo.claimed != null)
			lizzo.claimed.Unclaim();

		// Establish linkage
        Core.theTM.RemoveFromUnclaimed(this);
        isClaimed = true;
        reservee = lizzo;
		reservee.claimed = this;

    }

    public void Destroy()
    {
		// Drop it
		Drop();

		// Undo claims
		if (isClaimed)
			Unclaim();

        Core.theTM.allResources.Remove(this);
        if (!isClaimed)
            Core.theTM.RemoveFromUnclaimed(this);
        Destroy(gameObject);
    }

}
