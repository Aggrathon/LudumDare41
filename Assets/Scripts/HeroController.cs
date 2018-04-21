using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour {

	public float heroMoveTime = 0.7f;
	public Transform spawnPoint;
	public Transform[] tiles;
	HeroPos[] heroes;

	private void Awake()
	{
		heroes = new HeroPos[tiles.Length];
	}

	public void SpawnHero(Hero hero)
	{
		hero.transform.position = spawnPoint.position;
		hero.gameObject.SetActive(true);
		int pos = 0;
		while(pos < heroes.Length && (heroes[pos].hero != null || CheckPosition(pos)))
			pos++;
		if (pos == heroes.Length)
		{
			Debug.LogError("Could not spawn " + hero);
			return;
		}
		heroes[pos] = new HeroPos(hero);
		StartCoroutine(MovingHero(pos));
	}

	public bool MoveHero(Hero hero, int pos, Action<int> onFinishedMoving = null)
	{
		if (heroes[pos].hero != null)
			return false;
		for (int i = 0; i < heroes.Length; i++)
		{
			if (heroes[i].hero == hero)
			{
				heroes[i].hero = null;
				heroes[pos] = new HeroPos(hero);
				StartCoroutine(MovingHero(pos, onFinishedMoving));
				return true;
			}
		}
		return false;
	}

	IEnumerator MovingHero(int pos, Action<int> onFinishedMoving=null)
	{
		heroes[pos].hero.sprite.sortingOrder = tiles.Length - pos;
		while ((tiles[pos].position - heroes[pos].hero.transform.position).sqrMagnitude > 0.01f)
		{
			heroes[pos].hero.transform.position = Vector3.SmoothDamp(
				heroes[pos].hero.transform.position,
				tiles[pos].position,
				ref heroes[pos].easeInOut,
				heroMoveTime
				);
			yield return null;
			if (heroes[pos].hero == null)
				yield break;
		}
		heroes[pos].hero.sprite.sortingOrder = 0;
		if (CheckPosition(pos))
		{
			//TODO: Kill the units
		}
		else
		{
			if (onFinishedMoving != null)
				onFinishedMoving(pos);
		}
	}

	public void StartTurn()
	{
		for (int i = 0; i < heroes.Length; i++)
		{
			Hero h = heroes[i].hero;
			if (h != null && h.CanMove())
				SelectionMarkers.instance.AddMarker(tiles[i].position, () => { ShowMovement(h); });
		}
	}

	void ShowMovement(Hero hero)
	{
		for (int i = 0; i < heroes.Length; i++)
		{
			int j = i;
			if (heroes[i].hero == null)
				SelectionMarkers.instance.AddMarker(tiles[i].position, () => {
						if (!MoveHero(hero, j, hero.DoAction))
							ShowMovement(hero);
					});
		}
	}

	bool CheckPosition(int pos)
	{
		if (pos - 2 >= 0 && heroes[pos - 2].hero != null && heroes[pos - 1].hero != null)
			return true;
		else if (pos + 2 < heroes.Length && heroes[pos + 2].hero != null && heroes[pos + 1].hero != null)
			return true;
		else if (pos - 1 >= 0 && pos + 1 < heroes.Length && heroes[pos - 1].hero != null && heroes[pos + 1].hero != null)
			return true;
		return false;
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

	struct HeroPos
	{
		public Hero hero;
		public Vector3 easeInOut;

		public HeroPos (Hero h)
		{
			hero = h;
			easeInOut = Vector3.zero;
		}
	}

}
