using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : AAbility
{
	public LineRenderer lr;
	public ParticleSystem ps;
	public AudioSource au;

	public float beamSpeed = 3.0f;

	public override void DoAction(int lane)
	{
		var grid = GameState.instance.enemies.GetGrid();
		for (int j = 1; j < grid.GetLength(1); j++)
		{
			int i_ = lane;
			int j_ = j;
			SelectionMarkers.instance.AddMarker(grid[lane, j].position, () => {
				StartCoroutine(Action(i_, j_));
			});
		}
	}

	IEnumerator Action(int i, int j)
	{
		var grid = GameState.instance.enemies.GetGrid();
		Vector3 pos = transform.position;
		if (au != null)
			au.Play();
		yield return null;
		//LINE FX
		lr.gameObject.SetActive(true);
		lr.SetPosition(0, pos);
		float t = 0f;
		do
		{
			t += Time.deltaTime * beamSpeed;
			float y = 1.0f - (t - 0.5f) * (t - 0.5f) * 4f;
			pos = Vector3.Lerp(transform.position, grid[i, j].position, t);
			pos.y += y*0.8f;
			int n = Mathf.CeilToInt(t * 8f);
			lr.positionCount = n + 1;
			lr.SetPosition(n, pos);
			au.transform.position = pos;
			yield return null;
		} while (t < 1.0f);
		//Prticle FX
		if (ps != null)
		{
			ps.transform.position = grid[i, j].position;
			ps.Play();
		}
		yield return new WaitForSeconds(0.1f);
		//PUSH
		for (int x = i - 1; x >= 0; x--)
			if (grid[x, j].enemy == null) {
				for (int y = x + 1; y < i; y++)
					GameState.instance.enemies.Move(y, j, y - 1, j);
				break;
			}
		for (int x = j - 1; x >= 0; x--)
			if (grid[i, x].enemy == null) {
				for (int y = x + 1; y < j; y++)
					GameState.instance.enemies.Move(i, y, i, y - 1);
				break;
			}
		for (int x = i + 1; x < grid.GetLength(0); x++)
			if (grid[x, j].enemy == null) {
				for (int y = x - 1; y > i; y--)
					GameState.instance.enemies.Move(y, j, y + 1, j);
				break;
			}
		for (int x = j + 1; x < grid.GetLength(1); x++)
			if (grid[i, x].enemy == null) {
				for (int y = x - 1; y > j; y--)
					GameState.instance.enemies.Move(i, y, i, y + 1);
				break;
			}
		yield return new WaitForSeconds(0.1f);
		//KILL
		if (grid[i, j].enemy != null)
			grid[i, j].enemy.Die();
		lr.gameObject.SetActive(false);
		yield return new WaitForSeconds(0.6f);
		GameState.instance.enemies.CheckThreeInRow();
		GameState.instance.EnemyTurn();
	}
}
