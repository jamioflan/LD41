using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hatchery : TileBase {

    public override TileBase.TileType Type()
    {
        return TileBase.TileType.HATCHERY;
    }

	public override bool CanBeBuiltOver() { return false; }
	public override bool CanBeDug() { return false; }
	public override bool CanBeFilledIn() { return true; }
	public override bool CanBeMarkedAsPriority() { return true; }

	public override void Start()
    {
        base.Start();
    }


    private float fGestationTimeRemaining = 1.0f;

    public override void Update()
    {
        base.Update();

        if (x < 0 || y < 0)
            return;

		// Where's the foetus gonna gestate? Temporary code
		if (fGestationTimeRemaining > 0.0f)
		{
			fGestationTimeRemaining -= Time.deltaTime;
		}
        if (fGestationTimeRemaining <= 0.0f)
        {
			// Check capacity
			int capacity = Core.theTM.GetNumTilesOfType(TileType.NEST) * Nest.lizardCapacity;
			int currentNumLizards = 0;
			foreach (List<Lizard> llist in Core.theTM.lizards.Values)
			{
				currentNumLizards += llist.Count;
			}

			if ( currentNumLizards < capacity  )
			{
				fGestationTimeRemaining = 1.0f;
				SpawnLizard();
			}
        }
    }

    private void SpawnLizard()
    {
        Lizard lizzie = Core.theTM.CreateLizard(x, y);

        lizzie.lizardName = lizardNames[Random.Range(0, lizardNames.Length)];
        TextTicker.AddLine(lizzie.lizardName + " has hatched");
    }

    public string[] lizardNames;
}
