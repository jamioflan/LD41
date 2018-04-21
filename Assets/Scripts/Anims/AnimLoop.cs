using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimLoop : Anim
{
    public Sprite[] sprites;
    public float fTimePerFrame = 1.0f;

    public override Sprite GetSprite(float fTime)
    {
        float fParametric = fTime / fTimePerFrame;
        return sprites[Mathf.FloorToInt(fParametric) % sprites.Length];
    }
}
