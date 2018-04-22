using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public GameObject deathFX;

	public void Die()
	{
		var ps = ObjectPool.Spawn(deathFX, transform.position).GetComponent<ParticleSystem>();
		ps.Stop();
		ps.Play();
		GameState.instance.kills++;
		Destroy(gameObject);
	}
}
