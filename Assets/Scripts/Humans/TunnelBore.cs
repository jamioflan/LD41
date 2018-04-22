using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelBore : Entity
{
    public float fSpeed = 1.0f;
    public int iCurrentX = -100;
    public int iDepth = 0;

    public override void Update()
    {
        base.Update();

        transform.localPosition += new Vector3(Time.deltaTime * fSpeed * (bFlip ? -1.0f : 1.0f), 0.0f, 0.0f);

        if (transform.position.x > 12.0f || transform.position.x < -12.0f)
            Destroy(gameObject);

        int iDrillProgress = Mathf.FloorToInt(transform.localPosition.x + (TileManager.width / 2 + 0.5f));

        if(iDrillProgress != iCurrentX)
        {
            iCurrentX = iDrillProgress;

            if(iCurrentX >= 0 && iCurrentX < TileManager.width)
            {
                if (!Core.theTM.HumanDigTile(iCurrentX, iDepth))
                {
                    CeaseDrilling();
                }
            }
        }
    }

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        for(int i = 0; i < TileManager.width; i++)
        {
            TileBase tb = Core.theTM.tiles[i, iDepth];
            tb.bWarning++;

            if (tb.IsLizardy())
            {
                TextTicker.AddLine("Warning: Humans are about to drill into our " + tb.printName);
            }
        }
    }

    public void CeaseDrilling()
    {
        fSpeed *= 4.0f;
        SetLeft(!bFlip);
    }
}
