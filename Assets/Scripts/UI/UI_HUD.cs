﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HUD : MonoBehaviour
{
	public Slider humanSuspicionMeter;
	public GameObject toolbarGroup;
	public GameObject buildOptionsGroup;
	public GameObject shopOptionsGroup;
	public Text numMetal;
	public Text numGems;
	public Text numMushrooms;
	public Text numMoney;
	public Text numLizardsDisguisedAsHumans;

	public enum BUILD_ITEM
	{
		STORAGE = 0,
		HATCHERY = 1,
		NEST = 2,
		TAILOR = 3,
	}

	// Whether we've selected an item to place, but haven't placed it yet
	bool isBuildingAThing;
	BUILD_ITEM thingToBuild;

	// Whether we've chosen to dig a tile, but haven't chosen which yet
	bool isDiggingATile;

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
		// Update the counters
		if (humanSuspicionMeter != null)
		{
			humanSuspicionMeter.value = Player.thePlayer.fHumanSuspicion;
		}
		if( numMetal != null )
		{
			numMetal.text = "" + Player.thePlayer.metal;
		}
		if (numGems != null)
		{
			numGems.text = "" + Player.thePlayer.gems;
		}
		if (numMushrooms != null)
		{
			numMushrooms.text = "" + Player.thePlayer.mushrooms;
		}
		if( numMoney != null )
		{
			numMoney.text = "" + Player.thePlayer.money;
		}
		if( numLizardsDisguisedAsHumans != null)
		{
			numLizardsDisguisedAsHumans.text = "" + Player.thePlayer.lizardsDisguisedAsHumans;
		}

		// Hide all highlights
		for (int ii = 0; ii < TileManager.width; ++ii)
		{
			for (int jj = 0; jj < TileManager.depth; ++jj)
			{
				TileBase thisTile = Core.theTM.tiles[ii, jj];
				thisTile.bShouldBeHighlighted = false;
			}
		}

		if ( isBuildingAThing || isDiggingATile || isFillingInATile || isMarkingATileAsPriority )
		{
			// Get the tile that the mouse is over (if any!)
			TileBase mousedOverTile = null;
			bool bHasClicked = (Input.GetAxis("Fire1") > 0.0f);

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
			if (hit)
			{
				mousedOverTile = hit.collider.gameObject.GetComponent<TileBase>();

				if( bHasClicked )
				{
					Debug.Log("Clicked on " + hit.collider.gameObject.name);
				}
			}

			// Iterate through all tiles, check if they are valid for this action
			for (int ii = 0; ii < TileManager.width; ++ii)
			{
				for (int jj = 0; jj < TileManager.depth; ++jj)
				{
					TileBase thisTile = Core.theTM.tiles[ii, jj];

					if( ( isBuildingAThing && thisTile.CanBeBuiltOver() ) ||
						( isDiggingATile && thisTile.CanBeDug() ) ||
						( isFillingInATile && thisTile.CanBeFilledIn() ) ||
						( isMarkingATileAsPriority && thisTile.CanBeMarkedAsPriority() ) )
					{
						bool bValid = true;

						// If digging a tile, also need to check the tile is adjacent to some other lizardy tile.
						if( isDiggingATile )
						{
							TileBase tileOnLeft = null;
							if (ii - 1 >= 0)
							{
								tileOnLeft = Core.theTM.tiles[ii - 1, jj];
							}
							TileBase tileOnRight = null;
							if (ii + 1 < TileManager.width)
							{
								tileOnRight = Core.theTM.tiles[ii + 1, jj];
							}
							TileBase tileAbove = null;
							if (jj - 1 >= 0)
							{
								tileAbove = Core.theTM.tiles[ii, jj - 1];
							}
							TileBase tileBelow = null;
							if (jj + 1 < TileManager.depth)
							{
								tileBelow = Core.theTM.tiles[ii, jj + 1];
							}

							if( (tileOnLeft == null || !tileOnLeft.IsLizardy()) &&
								(tileOnRight == null || !tileOnRight.IsLizardy()) &&
								(tileAbove == null || !tileAbove.IsLizardy()) &&
								(tileBelow == null || !tileBelow.IsLizardy()) )
							{
								bValid = false;
							}
						}

						if (bValid)
						{
							// Highlight tile. Also render extra highlight if moused-over.
							thisTile.bShouldBeHighlighted = true;

							// If we're mousing over this tile...
							if (mousedOverTile != null && mousedOverTile == thisTile)
							{
								// Render transparent version of some image to indicate mouse over
								//Instantiate<Sprite>(highlightSprite);

								// If we clicked on this tile, do the thing!
								if (bHasClicked)
								{
									if (isBuildingAThing)
									{
										Debug.Log("Building a thing!");

										TileBase targetTile = hit.collider.gameObject.GetComponent<TileBase>();
										if (targetTile != null)
										{
											TileBase.TileType eTileType = GetTileTypeToBuild();
											Core.theTM.RequestNewTile(targetTile.x, targetTile.y, eTileType);
										}
									}
									else if (isDiggingATile)
									{
										Debug.Log("Digging a tile!");

										TileBase targetTile = hit.collider.gameObject.GetComponent<TileBase>();
										if (targetTile != null)
										{
											TileBase.TileType eTileType = TileBase.TileType.EMPTY;
											Core.theTM.RequestNewTile(targetTile.x, targetTile.y, eTileType);
										}
									}
									else if (isFillingInATile)
									{
										Debug.Log("Filling in a tile!");

										TileBase targetTile = hit.collider.gameObject.GetComponent<TileBase>();
										if (targetTile != null)
										{
											TileBase.TileType eTileType = TileBase.TileType.FILLED;
											Core.theTM.RequestNewTile(targetTile.x, targetTile.y, eTileType);
										}
									}
									else if (isMarkingATileAsPriority)
									{
										Debug.Log("Marking a tile as priority!");
									}
								}
							}
						}
					}
				}
			}

			if( bHasClicked )
			{
				// When anything is clicked, return everything to the default state
				Reset();
			}
		}
	}

	public void Reset()
	{
		isBuildingAThing = false;
		isDiggingATile = false;
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

		if (shopOptionsGroup != null)
		{
			shopOptionsGroup.SetActive(false);
		}
	}

	public void BuildSomething_OnClick()
	{
		if( buildOptionsGroup != null )
		{
			buildOptionsGroup.SetActive(!buildOptionsGroup.activeSelf);
		}

		if (shopOptionsGroup != null)
		{
			shopOptionsGroup.SetActive(false);
		}
	}

	public void DigSomething_OnClick()
	{
		if (buildOptionsGroup != null)
		{
			buildOptionsGroup.SetActive(false);
		}

		if (shopOptionsGroup != null)
		{
			shopOptionsGroup.SetActive(false);
		}

		// Hide the toolbar
		if (toolbarGroup != null)
		{
			toolbarGroup.SetActive(false);
		}

		isDiggingATile = true;
	}

	public void FillSomethingIn_OnClick()
	{
		if (buildOptionsGroup != null)
		{
			buildOptionsGroup.SetActive(false);
		}

		if (shopOptionsGroup != null)
		{
			shopOptionsGroup.SetActive(false);
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
			buildOptionsGroup.SetActive(false);
		}

		if (shopOptionsGroup != null)
		{
			shopOptionsGroup.SetActive(false);
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

		if (shopOptionsGroup != null)
		{
			shopOptionsGroup.SetActive(false);
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

	public void OpenTheShopOptions()
	{
		if (buildOptionsGroup != null)
		{
			buildOptionsGroup.SetActive(false);
		}

		if (shopOptionsGroup != null)
		{
			shopOptionsGroup.SetActive(!shopOptionsGroup.activeSelf);
		}
	}

	public void Shop_SellMetals()
	{
		// Sell a metal
		Player.thePlayer.SellMetal(1);

		// Return to normal
		if (toolbarGroup != null)
		{
			toolbarGroup.SetActive(true);
		}

		if (buildOptionsGroup != null)
		{
			buildOptionsGroup.SetActive(false);
		}

		if (shopOptionsGroup != null)
		{
			shopOptionsGroup.SetActive(false);
		}
	}

	public void Shop_SellGems()
	{
		// Sell a gem
		Player.thePlayer.SellGems(1);

		// Return to normal
		if (toolbarGroup != null)
		{
			toolbarGroup.SetActive(true);
		}

		if (buildOptionsGroup != null)
		{
			buildOptionsGroup.SetActive(false);
		}

		if (shopOptionsGroup != null)
		{
			shopOptionsGroup.SetActive(false);
		}
	}

	public void Shop_SellMushrooms()
	{
		// Sell a mushroom
		Player.thePlayer.SellMushrooms(1);

		// Return to normal
		if (toolbarGroup != null)
		{
			toolbarGroup.SetActive(true);
		}

		if (buildOptionsGroup != null)
		{
			buildOptionsGroup.SetActive(false);
		}

		if (shopOptionsGroup != null)
		{
			shopOptionsGroup.SetActive(false);
		}
	}

	public void Shop_BuyDrillEquipment()
	{
		// Buy some drill equipment...

		// Return to normal
		if (toolbarGroup != null)
		{
			toolbarGroup.SetActive(true);
		}

		if (buildOptionsGroup != null)
		{
			buildOptionsGroup.SetActive(false);
		}

		if (shopOptionsGroup != null)
		{
			shopOptionsGroup.SetActive(false);
		}
	}

	TileBase.TileType GetTileTypeToBuild()
	{
		switch( thingToBuild )
		{
			case BUILD_ITEM.STORAGE:
			{
				return TileBase.TileType.STORAGE;
			}
			case BUILD_ITEM.HATCHERY:
			{
				return TileBase.TileType.HATCHERY;
			}
			case BUILD_ITEM.NEST:
			{
				return TileBase.TileType.NEST;
			}
			case BUILD_ITEM.TAILOR:
			{
				return TileBase.TileType.TAILOR;
			}
			default:
			{
				return TileBase.TileType.FILLED;
			}
		}
	}
}
