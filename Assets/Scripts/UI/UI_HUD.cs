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
		// Update the human suspicion amount
		if (humanSuspicionMeter != null)
		{
			humanSuspicionMeter.value = 0.1f; // TODO: Get value!
		}

		if( isBuildingAThing || isFillingInATile || isMarkingATileAsPriority )
		{
			if( Input.GetAxis("Fire1") > 0.0f )
			{
				// When the mouse is clicked, we should do a raycast to check what tile was clicked on.
				// If none was clicked on, cancel the action and open the toolbar again. If one was
				// clicked on, perform the required action for that tile (and open the toolbar again!).


				// Return everything to the default state
				isBuildingAThing = false;
				isFillingInATile = false;
				isMarkingATileAsPriority = false;

				if (toolbarGroup != null)
				{
					toolbarGroup.SetActive(true);
				}

				if (buildOptionsGroup != null)
				{
					buildOptionsGroup.SetActive(false);
				}
			}
			else
			{
				// Maybe render an image to show the tile you're hovering over. Could be different image
				// per action/thing to build.
			}
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

		// Hide the toolbar
		if (toolbarGroup != null)
		{
			toolbarGroup.SetActive(false);
		}

		// Note down that the next click needs to do a thing
		isFillingInATile = true;
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

		// Store which thing we'll be building
		thingToBuild = (BUILD_ITEM)iThingIndex;
	}
}
