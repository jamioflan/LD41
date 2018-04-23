using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomFarm : TileBase {

    public override TileBase.TileType Type()
    {
        return TileBase.TileType.FARM;
    }

	public override Lizard.Assignment GetAss() { return Lizard.Assignment.FARMER; }
	public override int GetNumAss() { return 1; }

	public override bool CanBeBuiltOver() { return false; }
	public override bool CanBeDug() { return false; }
	public override bool CanBeFilledIn() { return true; }
	public override bool CanBeMarkedAsPriority() { return true; }

	public float fFarmProgress = 0.0f;

	public static readonly float fMUSHROOM_HARVEST_TIME = 25.0f;

	private Task task = null;

	public Resource mushroomPrefab;

	public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();

		if (task == null)
		{
			task = new Task(Task.Type.FARM);
			task.associatedTile = this;

			Player.thePlayer.pendingTasks[(int)Lizard.Assignment.FARMER].Add(task);
		}

		if(fFarmProgress >= fMUSHROOM_HARVEST_TIME)
		{
			int num = 1;
			if (Random.Range(0, 100) > 90)
				num += 2;
			for (int i = 0; i < num; i++)
			{
				Resource mushy = Instantiate<Resource>(mushroomPrefab);
				mushy.transform.position = transform.position + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), -1.0f);
				mushy.PutInRoom(this);
				Core.theTM.RegisterResource(mushy);
			}

			fFarmProgress = 0.0f;
			task = null;
		}
	}

	public bool Farm()
	{
		fFarmProgress += Time.deltaTime;

		SetTaskCompletion(fFarmProgress / fMUSHROOM_HARVEST_TIME);

		return fFarmProgress >= fMUSHROOM_HARVEST_TIME;
	}
}
