﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public GameObject deathFX;
	public Sprite secondLife;
	public SpriteRenderer spriteRenderer;

	public void Die()
	{
		if (secondLife != null)
		{
			spriteRenderer.sprite = secondLife;
			secondLife = null;
		}
		else
		{
			var ps = ObjectPool.Spawn(deathFX, transform.position).GetComponent<ParticleSystem>();
			ps.Stop();
			ps.Play();
			GameState.instance.kills++;
			Destroy(gameObject);
		}
	}
}
