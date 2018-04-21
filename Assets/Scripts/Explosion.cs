using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : AAbility
{
	int x, y;

	public override void SelectTarget()
	{
		var grid = GameState.instance.enemies.GetGrid();
		for (int i = 0; i < grid.GetLength(0); i++)
		{
			for (int j = 0; j < grid.GetLength(1); j++)
			{
				int i_ = i;
				int j_ = j;
				SelectionMarkers.instance.AddMarker(grid[i, j].position, () => {
					x = i_;
					y = j_;
					DoAction();
				});
			}
		}
	}

	protected override void DoAction()
	{
		var grid = GameState.instance.enemies.GetGrid();
		if (grid[x, y].enemy != null)
			grid[x, y].enemy.Die();

		GameState.instance.EnemyTurn();
	}
}
