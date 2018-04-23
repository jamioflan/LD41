using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plans : MonoBehaviour
{
	public int x, y;

	public Sprite sprite;

	public void InitSprites()
	{
		GetComponent<SpriteRenderer>().sprite = sprite;
		transform.position = new Vector3(-TileManager.width / 2 + x + 0.5f, -y - 0.5f, -3.0f);
	}
}
