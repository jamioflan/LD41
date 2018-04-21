using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Entity
{

    public float fSpeed = 1.0f;

    public override void Update()
    {
        base.Update();



        transform.localPosition += new Vector3(Time.deltaTime * fSpeed * (bFlip ? -1.0f : 1.0f), 0.0f, 0.0f);
    }
}
