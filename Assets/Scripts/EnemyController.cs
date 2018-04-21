using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	public Transform[] lanes;
	public GameObject enemyPrefab;
	public Transform spawnPoint;
	[Range(0, 1)] public float spawnDensity = 0.8f;
	public float enemyMoveTime = 0.4f;
	public float enemyMoveSpread = 0.05f;

	int[] permutation;
	EnemyPos[,] grid;
	int width;
	WaitForSeconds moveSpread;

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
				if (grid[i, j].enemy != null && grid[i, j - 1].enemy == null)
				{
					grid[i, j - 1].enemy = grid[i, j].enemy;
					grid[i, j - 1].easeInOut = grid[i, j].easeInOut;
					grid[i, j].enemy = null;
					StartCoroutine(MoveEnemy(i, j - 1));
					yield return moveSpread;
				}
			}
		}
		yield return moveSpread;
		for (int i = 0; i < lanes.Length; i++)
		{
			for (int j = 0; j < width; j++)
			{
				if (grid[i, j].enemy != null && CheckPosition(i, j))
				{
					//TODO: Kill the units
					Debug.LogError("Killing of units is not yet implemented");
				}
			}
		}
		for (int i = 0; i < lanes.Length; i++)
		{
			if (grid[i, 0].enemy != null)
			{
				GameState.instance.Loose();
				SpawnEnemies();
				yield break;
			}
		}
		SpawnEnemies();
		yield return moveSpread;
		GameState.instance.PlayerTurn();
	}

	public void SpawnEnemies()
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
				e.transform.position = spawnPoint.position;
				grid[rnd, width - 1].enemy = e;
				grid[rnd, width - 1].easeInOut = Vector3.zero;
				StartCoroutine(MoveEnemy(rnd, width - 1));
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

	IEnumerator MoveEnemy(int i, int j, bool check=false)
	{
		while ((grid[i, j].position - grid[i, j].enemy.transform.position).sqrMagnitude > 0.01f)
		{
			grid[i, j].enemy.transform.position = Vector3.SmoothDamp(
				grid[i, j].enemy.transform.position,
				grid[i, j].position,
				ref grid[i, j].easeInOut,
				enemyMoveTime
				);
			yield return null;
			if (grid[i, j].enemy == null)
				yield break;
		}
		if (check)
		{
			if (CheckPosition(i, j))
			{
				//TODO: Kill the units
				Debug.LogError("Killing of units is not yet implemented");
			}
		}
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
}
