using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroTiles : MonoBehaviour {

	public float heroMoveTime = 0.5f;
	public Transform spawnPoint;
	public Transform[] tiles;
	List<HeroPos> heroes;
	int heightIndex;

	private void Awake()
	{
		heroes = new List<HeroPos>();
		heightIndex = 0;
	}

	public void SpawnHero(Hero hero)
	{
		hero.transform.position = spawnPoint.position;
		hero.gameObject.SetActive(true);
		int pos = Random.Range(0, tiles.Length);
		while(CheckPosition(pos))
			pos = Random.Range(0, tiles.Length);
		hero.sprite.sortingOrder = -pos;
		heroes.Add(new HeroPos() { hero = hero, position = pos });
	}

	public bool MoveHero(Hero hero, int pos)
	{
		HeroPos hp = null;
		for (int i = 0; i < heroes.Count; i++)
		{
			if (heroes[i].position == pos)
				return false;
			if (heroes[i].hero == hero)
				hp = heroes[i];
		}
		if (hp == null)
			return false;
		hp.position = pos;
		hero.sprite.sortingOrder = ++heightIndex;
		if (CheckPosition(pos))
		{
			//TODO: Kill the units
		}
		return true;
	}

	private void Update()
	{
		for (int i = 0; i < heroes.Count; i++)
		{
			heroes[i].hero.transform.position = Vector3.SmoothDamp(
				heroes[i].hero.transform.position,
				tiles[heroes[i].position].position,
				ref heroes[i].easeInOut,
				heroMoveTime
				);
		}
	}

	bool CheckPosition(int pos)
	{
		bool prev = false;
		bool next = false;
		bool prev2 = false;
		bool next2 = false;
		for (int i = 0; i < heroes.Count; i++)
		{
			if (heroes[i].position - pos == 2)
				next2 = true;
			else if (heroes[i].position - pos == 1)
				next = true;
			else if (heroes[i].position - pos == -1)
				next = true;
			else if (heroes[i].position - pos == -2)
				next = true;
		}
		return (prev && prev2) || (prev && next) || (next && next2);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		for (int i = 0; i < tiles.Length; i++)
		{
			Gizmos.DrawWireCube(tiles[i].position, Vector3.one);
		}
		if (spawnPoint != null)
			Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
	}

	class HeroPos
	{
		public Hero hero;
		public int position;
		public Vector3 easeInOut;
	}

}
