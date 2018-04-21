using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimNonLoop : Anim
{
    public Sprite[] sprites;
    public float fTimePerFrame = 1.0f;

    public override Sprite GetSprite(float fTime)
    {
        float fParametric = fTime / fTimePerFrame;
        return sprites[Mathf.Clamp(Mathf.FloorToInt(fParametric), 0, sprites.Length - 1)];
    }
}
