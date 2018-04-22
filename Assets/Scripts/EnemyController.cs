using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour {

	public Transform[] lanes;
	public EnemyWave[] spawnWaves;
	public Transform spawnPoint;
	public float enemyMoveTime = 0.4f;
	public float enemyMoveSpread = 0.05f;
	public GameObject rewardText;

	int[] permutation;
	EnemyPos[,] grid;
	int width;
	WaitForSeconds moveSpread;
	List<Enemy> cache;

	//Ugly hack to make archers work;
	[System.NonSerialized] public int blockedI, blockedJ;

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
		moveSpread = new WaitForSeconds(enemyMoveSpread);
		cache = new List<Enemy>();
		blockedI = -5;
		blockedJ = -5;
	}

	public void StartTurn()
	{
		StartCoroutine(DoTurn());
	}

	IEnumerator DoTurn()
	{
		for (int i = 0; i < lanes.Length; i++)
		{
			for (int j = 1; j < width; j++)
			{
				if (grid[i, j].enemy != null && grid[i, j - 1].enemy == null && !(i == blockedI && j-1 == blockedJ))
				{
					Move(i, j, i, j - 1);
					yield return moveSpread;
				}
			}
		}
		yield return moveSpread;
		SpawnEnemies();
		yield return new WaitForSeconds(0.2f);
		CheckThreeInRow();
		for (int i = 0; i < lanes.Length; i++)
		{
			if (grid[i, 0].enemy != null)
			{
				GameState.instance.Loose();
				SpawnEnemies();
				yield break;
			}
		}
		GameState.instance.PlayerTurn();
	}

	public void SpawnEnemies()
	{
		EnemyWave ew = new EnemyWave();
		for (int i = spawnWaves.Length-1; i >= 0; i--)
		{
			if (spawnWaves[i].startTurn <= GameState.instance.turns)
			{
				ew = spawnWaves[i];
				break;
			}
		}
		if (ew.enemies == null)
			return;
		for (int i = 0; i < permutation.Length; i++)
		{
			int tmp = permutation[i];
			int rnd = Random.Range(i, permutation.Length);
			permutation[i] = permutation[rnd];
			permutation[rnd] = tmp;
			if (grid[rnd, width-1].enemy == null && !CheckPosition(rnd, width-1)) {
				float spawn = Random.value;
				for (int j = 0; j < ew.enemies.Length; j++)
				{
					if (spawn < ew.enemies[j].probability)
					{
						var e = Instantiate<GameObject>(ew.enemies[j].prefab, transform).GetComponent<Enemy>();
						e.transform.position = spawnPoint.position;
						grid[rnd, width - 1].enemy = e;
						grid[rnd, width - 1].easeInOut = Vector3.zero;
						StartCoroutine(MoveEnemy(rnd, width - 1));
					}
					else
					{
						spawn -= ew.enemies[j].probability;
					}
				}
			}
		}
	}

	bool CheckPosition(int i, int j)
	{
		if (i > 1 && grid[i - 2, j].enemy != null && grid[i - 1, j].enemy != null)
			return true;
		else if (i < lanes.Length - 2 && grid[i + 2, j].enemy != null && grid[i + 1, j].enemy != null)
			return true;
		if (j > 1 && grid[i, j - 2].enemy != null && grid[i, j - 1].enemy != null)
			return true;
		else if (j < width - 2 && grid[i, j + 2].enemy != null && grid[i, j + 1].enemy != null)
			return true;
		if (i > 0 && i < lanes.Length - 1 && grid[i + 1, j].enemy != null && grid[i - 1, j].enemy != null)
			return true;
		if (j > 0 && j < width - 1 && grid[i, j + 1].enemy != null && grid[i, j - 1].enemy != null)
			return true;
		return false;
	}

	public void Move(int i, int j, int i_, int j_)
	{
		grid[i_, j_].enemy = grid[i, j].enemy;
		grid[i_, j_].easeInOut = grid[i, j].easeInOut;
		grid[i, j].enemy = null;
		StartCoroutine(MoveEnemy(i_, j_));
	}

	IEnumerator MoveEnemy(int i, int j)
	{
		while (grid[i, j].enemy != null && (grid[i, j].position - grid[i, j].enemy.transform.position).sqrMagnitude > 0.01f)
		{
			grid[i, j].enemy.transform.position = Vector3.SmoothDamp(
				grid[i, j].enemy.transform.position,
				grid[i, j].position,
				ref grid[i, j].easeInOut,
				enemyMoveTime
				);
			yield return null;
		}
	}

	public void CheckThreeInRow()
	{
		cache.Clear();
		for (int i = 0; i < lanes.Length; i++)
		{
			for (int j = 0; j < width; j++)
			{
				if (grid[i, j].enemy != null && CheckPosition(i, j))
				{
					cache.Add(grid[i, j].enemy);
				}
			}
		}
		for (int i = 0; i < cache.Count; i++)
		{
			ObjectPool.Spawn(rewardText, cache[i].transform.position).GetComponent<Text>().text = "+ " + cache.Count * 8;
			cache[i].Die();
		}
		GameState.instance.score += cache.Count*cache.Count*8;
	}

	public EnemyPos[,] GetGrid() { return grid; }

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
		if (spawnPoint != null)
			Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
	}

	public struct EnemyPos
	{
		public Vector3 position;
		public Enemy enemy;
		public Vector3 easeInOut;
	}

	[System.Serializable]
	public struct EnemyProbability
	{
		public GameObject prefab;
		[Range(0, 1)] public float probability;
	}

	[System.Serializable]
	public struct EnemyWave
	{
		public int startTurn;
		public EnemyProbability[] enemies;
	}
}
