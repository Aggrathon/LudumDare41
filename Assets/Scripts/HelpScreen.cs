using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpScreen : MonoBehaviour {

	public static HelpScreen instance { get; protected set; }

	public Text title;
	public Text description;

	int stack = -1;
	
	void Awake () {
		instance = this;
		gameObject.SetActive(false);
	}

	public void ShowHelpSources()
	{
		CancelHelp();
		stack = SelectionMarkers.instance.AddStack();
		var grid = GameState.instance.enemies.GetGrid();
		for (int i = 0; i < grid.GetLength(0); i++)
		{
			for (int j = 0; j < grid.GetLength(1); j++)
			{
				if(grid[i, j].enemy != null)
				{
					var e = grid[i, j].enemy;
					SelectionMarkers.instance.AddMarker(grid[i, j].position, () => { ShowHelp(e); }, new Color(0.3f, 0.3f, 1.0f, 0.7f));
				}
			}
		}
		var heroes = GameState.instance.heroes.GetHeroes();
		for (int i = 0; i < heroes.Length; i++)
		{
			if (heroes[i].hero != null)
			{
				var h = heroes[i].hero;
				SelectionMarkers.instance.AddMarker(heroes[i].hero.transform.position, () => { ShowHelp(h); }, new Color(0.3f, 0.3f, 1.0f, 0.7f));
			}
		}
	}

	public void CancelHelp()
	{
		if (stack != -1)
		{
			SelectionMarkers.instance.RemoveStack(stack);
			stack = -1;
		}
	}

	public void ShowHelp(IHelp ih)
	{
		title.text = ih.title;
		description.text = ih.description;
		gameObject.SetActive(true);
		CancelHelp();
	}
}

public interface IHelp
{
	string title { get; }
	string description { get; }
}
