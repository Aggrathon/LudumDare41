using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameState : MonoBehaviour {

	public static GameState instance { get; protected set; }
	public HeroController heroes;
	public EnemyController enemies;
	public float startDelay = 1.5f;
	public GameObject skipButton;
	public GameObject lostPanel;
	public Text statusText;

	string statusFormat;
	int _turns;
	int _kills;
	int _score;
	public int turns
	{
		get { return _turns; }
		set { _turns = value; score += 100; }
	}
	public int kills
	{
		get { return _kills; }
		set { _kills = value; score += 25; }
	}
	public int score
	{
		get { return _score; }
		set {
			if (_score != value)
			{
				_score = value;
				UpdateStatus();
			}
		}
	}


	void Start ()
	{
		instance = this;
		StartCoroutine(DelayedStart());
		_turns = 0;
		_kills = 0;
		_score = 0;
		statusFormat = statusText.text;
		UpdateStatus();
		skipButton.SetActive(false);
		lostPanel.SetActive(false);
	}

	IEnumerator DelayedStart()
	{
		enemies.SpawnEnemies();
		yield return new WaitForSeconds(startDelay);
		PlayerTurn();
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

	public void UpdateStatus()
	{
		statusText.text = string.Format(statusFormat, turns, kills, score);
	}
}
