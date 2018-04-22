using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge : AAbility {

	public float chargeSpeed = 20.0f;

	public override void DoAction(int lane)
	{
		var grid = GameState.instance.enemies.GetGrid();
		for (int j = 0; j < grid.GetLength(1); j++)
		{
			if (grid[lane, j].enemy != null)
			{
				StartCoroutine(Action(lane, j));
				return;
			}
		}
		GameState.instance.EnemyTurn();
	}

	IEnumerator Action(int i, int j)
	{
		var grid = GameState.instance.enemies.GetGrid();
		Vector3 target = transform.InverseTransformPoint(grid[i, j].position);
		Vector3 dist = (target - transform.localPosition);
		float length = dist.magnitude;
		while (length > 1.2f)
		{
			transform.localPosition += dist * (chargeSpeed * Time.deltaTime / length);
			yield return null;
			dist = (target - transform.localPosition);
			length = dist.magnitude;
		}
		grid[i, j].enemy.Die();
		for (int x = j+1; ; x++)
		{
			if (x == grid.GetLength(1))
			{
				if (j == x - 1)
					break;
				else
				{
					x--;
					grid[i, x].enemy.Die();
					x--;
					for (; x > j; x--)
					{
						GameState.instance.enemies.Move(i, x, i, x + 1);
					}
					break;
				}
			}
			else if(grid[i, x].enemy == null)
			{
				x--;
				for (; x > j; x--)
				{
					GameState.instance.enemies.Move(i, x, i, x + 1);
				}
				break;
			}
		}
		while (length > 0.1f)
		{
			transform.localPosition += dist * (chargeSpeed * Time.deltaTime / length);
			yield return null;
			dist = (target - transform.localPosition);
			length = dist.magnitude;
		}
		length = target.magnitude;
		dist = Vector3.zero;
		while (transform.localPosition.sqrMagnitude > 0.1)
		{
			transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref dist, 0.5f, chargeSpeed*0.5f);
			yield return null;
		}
		GameState.instance.enemies.CheckThreeInRow();
		GameState.instance.EnemyTurn();
	}
}
