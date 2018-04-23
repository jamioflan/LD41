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

	private Task[] breedTasks = new Task[2];
	private float[] fBreedProgress = new float[2];

    private float fGestationTimeRemaining = 1.0f;

	public bool Breed(Lizard breeder)
	{
		for (int i = 0; i < 2; i++)
		{
			if (breeder.currentTask == breedTasks[i])
			{
				fBreedProgress[i] += Time.deltaTime;
				return IsDone();
			}
		}
		return false;
	}

	public bool IsDone()
	{
		return fBreedProgress[0] > 10.0f && fBreedProgress[1] > 10.0f;
	}

	public override void Update()
    {
        base.Update();

        if (x < 0 || y < 0)
            return;

		for (int i = 0; i < 2; i++)
		{
			if (breedTasks[i] == null)
			{
				breedTasks[i] = new Task(Task.Type.BREED);
				breedTasks[i].associatedTile = this;

				Player.thePlayer.pendingWorkerTasks.Add(breedTasks[i]);
			}
		}

		if (IsDone())
		{
			fBreedProgress[0] = 0.0f;
			fBreedProgress[1] = 0.0f;

			// Check capacity
			int capacity = Core.theTM.GetNumTilesOfType(TileType.NEST) * Nest.lizardCapacity;
			int currentNumLizards = 0;
			foreach (List<Lizard> llist in Core.theTM.lizards.Values)
			{
				currentNumLizards += llist.Count;
			}

			if (currentNumLizards < capacity)
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
