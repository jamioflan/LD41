using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public Anim currentAnim;
    public float fAnimTime = 0.0f;
    public bool bFlip = false;

    public virtual void Start () {
		if(bFlip)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
	}

    public virtual void Update ()
    {

        fAnimTime += Time.deltaTime;

        GetComponent<SpriteRenderer>().sprite = currentAnim.GetSprite(fAnimTime);

    }

    public void SetAnim(Anim anim)
    {
        currentAnim = anim;
        fAnimTime = 0.0f;
    }
}
