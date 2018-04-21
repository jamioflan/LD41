using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HUD : MonoBehaviour
{
	public Slider humanSuspicionMeter;
	public GameObject buildOptionsGroup;

	public enum BUILD_ITEM
	{
		CORRIDOR_HORIZONTAL,
		CORRIDOR_VERTICAL,
	}

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (humanSuspicionMeter != null)
		{
			// Set human suspicion amount
			humanSuspicionMeter.value = 0.1f; // TODO: Get value!
		}
	}

	public void BuildSomething_OnClick()
	{
		if( buildOptionsGroup != null )
		{
			buildOptionsGroup.SetActive(true);
		}
	}

	public void FillSomethingIn_OnClick()
	{
		if (buildOptionsGroup != null)
		{
			// This definitely shouldn't be active
			buildOptionsGroup.SetActive(false);
		}

		// Tell something somewhere that the next click needs to do a filling in
	}

	public void MarkSomethingAsPriority()
	{
		if (buildOptionsGroup != null)
		{
			// This definitely shouldn't be active
			buildOptionsGroup.SetActive(false);
		}

		// Tell something somewhere that the next click needs to do a priority marking
	}

	public void BuildASpecificThing( int iThingIndex )
	{
		if (buildOptionsGroup != null)
		{
			buildOptionsGroup.SetActive(false);
		}

		// Get the thing corresponding to this index
		BUILD_ITEM eItem = (BUILD_ITEM)iThingIndex;

		// Build the thing whenever the user next clicks on a tile (render mouse over image would be nice)
	}
}
