using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextTicker : MonoBehaviour
{
    public Text[] entries = new Text[10];

	public string[] hints;
	public float fTimeToNextHint = 30.0f;

    public static TextTicker INSTANCE ;

    void Start ()
    {
        INSTANCE = this;
    }
	
	void Update ()
    {
		fTimeToNextHint -= Time.deltaTime;
		if(fTimeToNextHint <= 0.0f)
		{
			fTimeToNextHint = 30.0f;
			AddLine("<color=blue>Hint: " + hints[Random.Range(0, hints.Length)] + "</color>");

		}
	}

	public static void AddLine(string line)
	{
		for (int i = 0; i < INSTANCE.entries.Length - 1; i++)
		{
			INSTANCE.entries[i].text = INSTANCE.entries[i + 1].text;
		}

		INSTANCE.entries[INSTANCE.entries.Length - 1].text = line;
	}
}
