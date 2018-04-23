using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : TileBase {

    public override TileBase.TileType Type()
    {
        return TileBase.TileType.TRAP;
    }

	public override bool CanBeBuiltOver() { return false; }
	public override bool CanBeDug() { return false; }
	public override bool CanBeFilledIn() { return false; }
	public override bool CanBeMarkedAsPriority() { return true; }

	public override Lizard.Assignment GetAss() { return Lizard.Assignment.TRAP; }
	public override int GetNumAss() { return 1; }

	private static readonly float fTRAP_TIME = 30.0f;

	public float fFarmProgress = 0.0f;
	private Task task = null;

	public Resource skinPrefab;

	public override void Start()
    {
        base.Start();
    }

	public override void Update()
	{
		base.Update();

		if (task == null)
		{
			task = new Task(Task.Type.TRAP);
			task.associatedTile = this;

			Player.thePlayer.pendingTasks[(int)Lizard.Assignment.TRAP].Add(task);
		}

		if (fFarmProgress >= fTRAP_TIME)
		{
			int num = 1;
			for (int i = 0; i < num; i++)
			{
				Resource skin = Instantiate<Resource>(skinPrefab);
				skin.transform.position = transform.position + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), -1.0f);
				skin.PutInRoom(this);
				Core.theTM.RegisterResource(skin);
			}

			fFarmProgress = 0.0f;
			task = null;
		}
	}

	public bool Farm()
	{
		fFarmProgress += Time.deltaTime;

		SetTaskCompletion(fFarmProgress / fTRAP_TIME);

		return fFarmProgress >= fTRAP_TIME;
	}
}
