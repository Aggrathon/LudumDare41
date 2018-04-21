using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour {

	public SpriteRenderer sprite;

	void Start () {
		if (sprite == null)
			sprite = GetComponent<SpriteRenderer>();
		GetComponentInParent<HeroTiles>().SpawnHero(transform);
	}
}
