using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour {

	public static GameState instance { get; protected set; }
	public HeroController heroes;
	public float startDelay = 1.5f;

	int turns;

	void Start ()
	{
		instance = this;
		StartCoroutine(DelayedStart());
		turns = 0;
	}

	IEnumerator DelayedStart()
	{
		yield return new WaitForSeconds(startDelay);
		PlayerTurn();
	}

	public void EnemyTurn()
	{
		turns++;
		//TODO: Enemy turn
		PlayerTurn();
	}

	public void PlayerTurn()
	{
		heroes.StartTurn();
	}
}
