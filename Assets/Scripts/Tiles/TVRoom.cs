using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVRoom : TileBase {

    public override TileBase.TileType Type()
    {
        return TileBase.TileType.TVROOM;
    }

	public override bool CanBeBuiltOver() { return false; }
	public override bool CanBeDug() { return false; }
	public override bool CanBeFilledIn() { return true; }
	public override bool CanBeMarkedAsPriority() { return false; }

	public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();

		fTimeUntilNextTVBill -= Time.deltaTime;
		if(fTimeUntilNextTVBill <= 0.0f)
		{
			fTimeUntilNextTVBill = 120.0f;
			if(fTVBill >= 1.0f)
			{
				int iBill = Mathf.FloorToInt(fTVBill);
				if(iBill <= Player.thePlayer.money)
				{
					Player.thePlayer.money -= iBill;
					TextTicker.AddLine("<color=green>You paid your TV bill of $" + iBill + "</color>");
				}
				else
				{
					Player.thePlayer.AddSuspicion((iBill - Player.thePlayer.money) * 2.0f);
					Player.thePlayer.money = 0;
					TextTicker.AddLine("<color=red>You failed to pay your TV bill of $" + iBill + "</color>");
					TextTicker.AddLine("<color=red>Human suspicion has increased</color>");
				}
				fTVBill = 0.0f;
			}
		}
    }

	public float fTimeUntilNextTVBill = 120.0f;
	public static float fTVBill = 0.0f;

	public void IncrementTVBill(float fAmount)
	{
		fTVBill += fAmount;
	}
}
