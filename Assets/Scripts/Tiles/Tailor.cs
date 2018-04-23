using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tailor : TileBase {

    public override TileBase.TileType Type()
    {
        return TileBase.TileType.TAILOR;
    }

	public override bool CanBeBuiltOver() { return false; }
	public override bool CanBeDug() { return false; }
	public override bool CanBeFilledIn() { return true; }
	public override bool CanBeMarkedAsPriority() { return true; }

	public override Lizard.Assignment GetAss() { return Lizard.Assignment.TAILOR; }
	public override int GetNumAss() { return 1; }

	private static readonly float fTAILOR_TIME = 60.0f;

	public float fSkinProgress = 0.0f;
	private Task task = null;

	public Lizard lizardToSend;

	public override void Start()
    {
        base.Start();
    }

	public override void Update()
	{
		base.Update();

		if (task == null)
		{
			task = new Task(Task.Type.TAILOR);
			task.associatedTile = this;
			task.requiredResources.Add(Resource.ResourceType.HUMAN_SKIN, 1);

			Player.thePlayer.pendingTasks[(int)Lizard.Assignment.TAILOR].Add(task);
		}

		if (fSkinProgress >= fTAILOR_TIME)
		{
			if(lizardToSend != null)
			{
				TextTicker.AddLine(lizardToSend.lizardName + " has donned human skin");
				TextTicker.AddLine("They will now inflitrate Humanstown");
				GetComponent<AudioSource>().Play();
				if (Player.thePlayer.lizardsDisguisedAsHumans == 0)
					Player.thePlayer.firstLizard = lizardToSend.lizardName;
				Player.thePlayer.lizardsDisguisedAsHumans++;
				Core.theTM.lizards[lizardToSend.assignment].Remove(lizardToSend);
				Destroy(lizardToSend.gameObject);
			}

			fSkinProgress = 0.0f;
			task = null;
		}
	}

	public bool Farm()
	{
		fSkinProgress += Time.deltaTime;

		lizardToSend = task.assignedLizard;

		SetTaskCompletion(fSkinProgress / fTAILOR_TIME);

		return fSkinProgress >= fTAILOR_TIME;
	}
}
