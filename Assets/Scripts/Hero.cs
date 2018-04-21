using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour {

	public SpriteRenderer sprite;
	public int cooldown = 1;
	int cd;

	void Start () {
		if (sprite == null)
			sprite = GetComponent<SpriteRenderer>();
		GetComponentInParent<HeroController>().SpawnHero(this);
		cd = 0;
	}

	public void DoAction(int lane)
	{
		//TODO: Hero Actions
		cd = cooldown;
		GameState.instance.EnemyTurn();
	}

	public bool CanMove()
	{
		if (cd > 0)
		{
			cd--;
			return false;
		}
		else
		{
			return true;
		}
	}
}
