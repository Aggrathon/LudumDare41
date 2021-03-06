﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : AAbility
{
	public AudioSource au;
	public AudioClip[] sounds;
	public float arrowSpeed = 20.0f;

	Vector3 target;
	Vector3 start;
	float scale;
	bool firstShot = false;
	bool hit = false;
	float t = 0f;

	public override void DoAction(int lane)
	{
		var grid = GameState.instance.enemies.GetGrid();
		for (int j = 1; j < grid.GetLength(1); j++)
		{
			int i_ = lane;
			int j_ = j;
			SelectionMarkers.instance.AddMarker(grid[lane, j].position, () => {
				target = grid[i_, j_].position;
				start = transform.parent.position;
				scale = arrowSpeed / (target - start).magnitude;
				GameState.instance.enemies.blockedI = i_;
				GameState.instance.enemies.blockedJ = j_;
				if (au != null)
					au.PlayOneShot(sounds[Random.Range(0, sounds.Length)]);
				gameObject.SetActive(true);
				firstShot = true;
				hit = false;
				t = 0f;
			});
		}
	}

	private void Update()
	{
		t += Time.deltaTime * scale;
		if (t > 1.0f)
		{
			if (!hit)
			{
				if (GameState.instance.enemies.GetGrid()[GameState.instance.enemies.blockedI, GameState.instance.enemies.blockedJ].enemy != null)
					GameState.instance.enemies.GetGrid()[GameState.instance.enemies.blockedI, GameState.instance.enemies.blockedJ].enemy.Die();
				hit = true;
			}
			if (firstShot)
			{
				GameState.instance.EnemyTurn();
				firstShot = false;
			}
			if (t > 1.2f)
			{
				start = transform.parent.position;
				scale = arrowSpeed / (target - start).magnitude;
				t = 0f;
				transform.position = transform.parent.position;
				hit = false;
			}
			else
			{
				transform.position = target;
			}
		}
		else
		{
			float y = 1.0f - (t - 0.5f) * (t - 0.5f) * 4f;
			Vector3 pos = Vector3.Lerp(start, target, t);
			pos.y += y * 1.2f / scale;
			transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(pos.y - transform.position.y, pos.x - transform.position.x) * Mathf.Rad2Deg);
			transform.position = pos;
		}
	}

	private void OnDisable()
	{
		GameState.instance.enemies.blockedI = -5;
		GameState.instance.enemies.blockedJ = -5;
	}
}
