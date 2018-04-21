using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HUD : MonoBehaviour
{
	public Slider humanSuspicionMeter;
	public GameObject toolbarGroup;
	public GameObject buildOptionsGroup;

	public enum BUILD_ITEM
	{
		CORRIDOR_HORIZONTAL,
		CORRIDOR_VERTICAL,
	}

	// Whether we've selected an item to place, but haven't placed it yet
	bool isBuildingAThing;
	BUILD_ITEM thingToBuild;

	// Whether we've chosen to fill in a tile, but haven't chosen which yet
	bool isFillingInATile;

	// Whether we've chosen to mark a tile as priority, but haven't chosen which yet
	bool isMarkingATileAsPriority;

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
			buildOptionsGroup.SetActive(!buildOptionsGroup.activeSelf);
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

		// Hide the toolbar
		if (toolbarGroup != null)
		{
			toolbarGroup.SetActive(false);
		}

		// Note down that the next click needs to do a thing
		isMarkingATileAsPriority = true;
	}

	public void BuildASpecificThing( int iThingIndex )
	{
		if (buildOptionsGroup != null)
		{
			buildOptionsGroup.SetActive(false);
		}

		// Hide the toolbar
		if (toolbarGroup != null)
		{
			toolbarGroup.SetActive(false);
		}

		// Note down that the next click needs to do a thing
		isBuildingAThing = true;

		// Get the thing corresponding to this index
		thingToBuild = (BUILD_ITEM)iThingIndex;

		// Build the thing whenever the user next clicks on a tile (render mouse over image would be nice)
	}
}
