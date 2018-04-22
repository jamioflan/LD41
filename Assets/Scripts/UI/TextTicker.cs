using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextTicker : MonoBehaviour
{
    public Text[] entries = new Text[10];

    public static TextTicker INSTANCE ;

    void Start ()
    {
        INSTANCE = this;
    }
	
	void Update ()
    {
		
	}

    public static void AddLine(string line)
    {
        for(int i = 0; i < INSTANCE.entries.Length - 1; i++)
        {
            INSTANCE.entries[i].text = INSTANCE.entries[i + 1].text;
        }

        INSTANCE.entries[INSTANCE.entries.Length - 1].text = line;
    }
}
