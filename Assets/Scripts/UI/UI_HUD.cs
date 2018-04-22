using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HUD : MonoBehaviour
{
	public Slider humanSuspicionMeter;
	public GameObject toolbarGroup;
	public GameObject buildOptionsGroup;
	public GameObject shopOptionsGroup;

	public Sprite highlightSprite;
	Sprite[] activeHighlightSprites;

	public enum BUILD_ITEM
	{
		STORAGE = 0,
		HATCHERY = 1,
		NEST = 2,
		TAILOR = 3,
	}

	TileManager theTileManager;

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
		theTileManager = Core.theCore.GetComponent<TileManager>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Update the human suspicion amount
		if (humanSuspicionMeter != null)
		{
			humanSuspicionMeter.value = 0.1f; // TODO: Get value!
		}

		if( isBuildingAThing || isDiggingATile || isFillingInATile || isMarkingATileAsPriority )
		{
			// Iterate through all tiles, check if they are valid for this action
			for (int ii = 0; ii < TileManager.width; ++ii)
			{
				for (int jj = 0; jj < TileManager.depth; ++jj)
				{
					TileBase tile = theTileManager.tiles[ii, jj];
					if( ( isBuildingAThing && tile.CanBeBuiltOver() ) ||
						( isDiggingATile && tile.CanBeDug() ) ||
						( isFillingInATile && tile.CanBeFilledIn() ) ||
						( isMarkingATileAsPriority && tile.CanBeMarkedAsPriority() ) )
					{
						bool bValid = true;

						// If digging a tile, also need to check the tile is adjacent to some other lizardy tile.
						if( isDiggingATile )
						{
							TileBase tileOnLeft = null;
							if (ii - 1 >= 0)
							{
								tileOnLeft = theTileManager.tiles[ii - 1, jj];
							}
							TileBase tileOnRight = null;
							if (ii + 1 < TileManager.width)
							{
								tileOnRight = theTileManager.tiles[ii + 1, jj];
							}
							TileBase tileAbove = null;
							if (jj - 1 >= 0)
							{
								tileAbove = theTileManager.tiles[ii, jj - 1];
							}
							TileBase tileBelow = null;
							if (jj + 1 < TileManager.depth)
							{
								tileBelow = theTileManager.tiles[ii, jj + 1];
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
							//Instantiate<Sprite>(highlightSprite);
						}
					}
				}
			}

			if ( Input.GetAxis("Fire1") > 0.0f )
			{
				// When the mouse is clicked, we should do a raycast to check what tile was clicked on.
				// If none was clicked on, cancel the action and open the toolbar again. If one was
				// clicked on, perform the required action for that tile (and open the toolbar again!).
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
				if (hit)
				{
					Debug.Log("Hit " + hit.collider.gameObject.name);

					if( isBuildingAThing )
					{
						if( hit.collider.gameObject.name == "Empty(Clone)" )
						{
							Debug.Log("Building a thing!");

							if( theTileManager != null )
							{
								TileBase targetTile = hit.collider.gameObject.GetComponent<TileBase>();
								if( targetTile != null )
								{
									TileBase.TileType eTileType = GetTileTypeToBuild();
									theTileManager.RequestNewTile( targetTile.x, targetTile.y, eTileType);
								}
							}
						}
					}
					else if( isDiggingATile )
					{
						if (hit.collider.gameObject.name == "Filled(Clone)")
						{
							Debug.Log("Digging a tile!");

							if (theTileManager != null)
							{
								TileBase targetTile = hit.collider.gameObject.GetComponent<TileBase>();
								if (targetTile != null)
								{
									TileBase.TileType eTileType = TileBase.TileType.EMPTY;
									theTileManager.RequestNewTile(targetTile.x, targetTile.y, eTileType);
								}
							}
						}
					}
					else if( isFillingInATile )
					{
						if (hit.collider.gameObject.name == "Empty(Clone)")
						{
							Debug.Log("Filling in a tile!");

							if (theTileManager != null)
							{
								TileBase targetTile = hit.collider.gameObject.GetComponent<TileBase>();
								if (targetTile != null)
								{
									TileBase.TileType eTileType = TileBase.TileType.FILLED;
									theTileManager.RequestNewTile(targetTile.x, targetTile.y, eTileType);
								}
							}
						}
					}
					else if( isMarkingATileAsPriority )
					{
						Debug.Log("Marking a tile as priority!");
					}
				}

				// Return everything to the default state
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
			else
			{
				// Maybe render an image to show the tile you're hovering over. Could be different image
				// per action/thing to build.
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
		// Sell some metal...

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
		// Sell some gems...

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
		// Sell some mushrooms...

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
