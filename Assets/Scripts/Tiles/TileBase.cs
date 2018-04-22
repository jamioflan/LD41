using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class TileBase : MonoBehaviour {

    public int x;
    public int y;

    public SpriteRenderer warningSprite;
    private float fWarningTime = 0.0f;
    public int bWarning = 0; // Number of things intending to dig this tile
    public string printName = "";

    public enum TileType {
        EMPTY = 0,
        FILLED = 1,
        HUT = 2,
        STORAGE = 3,
        HATCHERY = 4,
        NEST = 5,
        TAILOR = 6,
        TRAP = 7,
        FARM = 8,
        TVROOM = 9
    }

    public abstract TileType Type();

    virtual public bool IsPassable()
    {
        return true;
    }

    public bool IsLizardy()
    {
        switch(Type())
        {
            case TileType.FILLED:
                return false;
        }
        return true;
    }

    public virtual void Start ()
    {
        warningSprite.enabled = false;
    }
	
	public virtual void Update ()
    {
        warningSprite.enabled = bWarning > 0;

        if (bWarning > 0)
        {
            fWarningTime += Time.deltaTime * 2.0f;

            if(warningSprite != null)
            {
                float scale = 1.0f + 0.1f * Mathf.Sin(fWarningTime);
                warningSprite.transform.localScale = new Vector3(scale, scale, scale);
            }
        }
	}

    public virtual void Destroy()
    {
		Destroy(gameObject);
    }


    public void SetCoords(int px, int py)
    {
        x = px;
        y = py;
        transform.position = new Vector3(x + 0.5f - TileManager.width / 2, -0.5f - y, -2);
    }

}
