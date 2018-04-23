using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hatchery : TileBase {

    public override TileBase.TileType Type()
    {
        return TileBase.TileType.HATCHERY;
    }

	public override Lizard.Assignment GetAss() { return Lizard.Assignment.HATCHERY; }
	public override int GetNumAss() { return 2; }

	public static readonly float fBREED_TIME = 45.0f;

	public override bool CanBeBuiltOver() { return false; }
	public override bool CanBeDug() { return false; }
	public override bool CanBeFilledIn() { return true; }
	public override bool CanBeMarkedAsPriority() { return true; }

	public override void Start()
    {
        base.Start();
    }

	private Task[] breedTasks = new Task[2];
	public float[] fBreedProgress = new float[2];

	public bool Breed(Lizard breeder)
	{
		for (int i = 0; i < 2; i++)
		{
			if (breeder.currentTask == breedTasks[i])
			{
				fBreedProgress[i] += Time.deltaTime;

				SetTaskCompletion(Mathf.Min(fBreedProgress[0], fBreedProgress[1]) / fBREED_TIME);

				return IsDone();
			}
		}
		return false;
	}


    public bool IsDone()
	{
		return fBreedProgress[0] > fBREED_TIME && fBreedProgress[1] > fBREED_TIME;
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

				Player.thePlayer.pendingTasks[(int)Lizard.Assignment.HATCHERY].Add(breedTasks[i]);
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

			//if (currentNumLizards < capacity)
			{
				SpawnLizard();
			}

			for (int i = 0; i < 2; i++)
			{
				breedTasks[i] = null;
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
