using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	public Transform[] lanes;
	public GameObject enemyPrefab;
	public Transform spawnPoint;
	[Range(0, 1)] public float spawnDensity = 0.8f;

	int[] permutation;
	EnemyPos[,] grid;
	int width;

	private void Start()
	{
		permutation = new int[lanes.Length];
		for (int i = 0; i < permutation.Length; i++)
			permutation[i] = i;
		width = lanes[0].childCount;
		grid = new EnemyPos[lanes.Length, width];
		for (int i = 0; i < lanes.Length; i++)
			for (int j = 0; j < width; j++)
				grid[i, j] = new EnemyPos() { position = lanes[i].GetChild(j).position };
	}

	public void StartTurn()
	{
		//TODO: Move enemies forward
		SpawnEnemies();
		GameState.instance.PlayerTurn();
	}

	void SpawnEnemies()
	{
		for (int i = 0; i < permutation.Length; i++)
		{
			int tmp = permutation[i];
			int rnd = Random.Range(i, permutation.Length);
			permutation[i] = permutation[rnd];
			permutation[rnd] = tmp;
			if (!CheckPosition(rnd, width-1) && Random.value <= spawnDensity)
			{
				var e = Instantiate<GameObject>(enemyPrefab, transform).GetComponent<Enemy>();
				grid[rnd, width - 1].enemy = e;
				//TODO: Move Enemy
				e.transform.position = grid[rnd, width - 1].position;
			}
		}
	}

	bool CheckPosition(int i, int j)
	{
		return false;
	}

	private void OnDrawGizmos()
	{
		if (lanes == null)
			return;
		Gizmos.color = Color.red;
		for (int i = 0; i < lanes.Length; i++)
		{
			if (lanes[i] == null)
				continue;
			for (int j = 0; j < lanes[i].childCount; j++)
			{
				Gizmos.DrawWireCube(lanes[i].GetChild(j).position, Vector3.one);
			}
		}
	}

	struct EnemyPos
	{
		public Vector3 position;
		public Enemy enemy;
	}
}
