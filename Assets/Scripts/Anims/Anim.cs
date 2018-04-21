using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Anim : MonoBehaviour
{
    public abstract Sprite GetSprite(float fTime);
}
