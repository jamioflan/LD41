using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public Anim currentAnim;
    public float fAnimTime = 0.0f;
    public bool bFlip = false;

    public AudioClip[] clips;
    public float fMinDelay = 5.0f, fMaxDelay = 10.0f, fDelay = 0.0f;

    public virtual void Start () {
		if(bFlip)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }

       // fDelay = Random.Range(fMinDelay, fMaxDelay);

    }

    public virtual void Update ()
    {

        fAnimTime += Time.deltaTime;

        GetComponent<SpriteRenderer>().sprite = currentAnim.GetSprite(fAnimTime);

        fDelay -= Time.deltaTime;
        if (fDelay <= 0.0f)
        {
            fDelay = Random.Range(fMinDelay, fMaxDelay);

            AudioSource source = GetComponent<AudioSource>();
            if (source != null)
            {
                source.clip = clips[Random.Range(0, clips.Length)];
                source.Play();
            }
        }
    }

    public void SetAnim(Anim anim)
    {
        if (anim != currentAnim)
        {
            currentAnim = anim;
            fAnimTime = 0.0f;
        }
    }

    public void SetLeft(bool bLeft)
    {
        bFlip = bLeft;
        GetComponent<SpriteRenderer>().flipX = bFlip;
    }
}
