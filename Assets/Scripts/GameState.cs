using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour {

	public static GameState instance { get; protected set; }
	public HeroController heroes;
	public EnemyController enemies;
	public float startDelay = 1.5f;
	public GameObject skipButton;
	public GameObject lostPanel;

	int turns;

	void Start ()
	{
		instance = this;
		StartCoroutine(DelayedStart());
		turns = 0;
		skipButton.SetActive(false);
		lostPanel.SetActive(false);
	}

	IEnumerator DelayedStart()
	{
		yield return new WaitForSeconds(startDelay);
		turns--;
		EnemyTurn();
	}

	public void EnemyTurn()
	{
		turns++;
		enemies.StartTurn();
		skipButton.SetActive(false);
	}

	public void PlayerTurn()
	{
		heroes.StartTurn();
		skipButton.SetActive(true);
	}

	public void Loose()
	{
		lostPanel.SetActive(true);
	}

	public void Restart()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
