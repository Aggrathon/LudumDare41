using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour {

	public SpriteRenderer sprite;
	public GameObject deathFX;
	int cd;

	public AAbility ability;

	void Start () {
		if (sprite == null)
			sprite = GetComponent<SpriteRenderer>();
		GetComponentInParent<HeroController>().SpawnHero(this);
		cd = 0;
	}

	public void Die()
	{
		var ps = ObjectPool.Spawn(deathFX, transform.position).GetComponent<ParticleSystem>();
		ps.Stop();
		ps.Play();
		Destroy(gameObject);
	}

	public void DoAction(int lane)
	{
		cd = ability.cooldown;
		ability.DoAction(lane);
	}

	public bool CanMove()
	{
		if (cd > 0)
		{
			cd--;
			return false;
		}
		else
		{
			return true;
		}
	}
}

public abstract class AAbility : MonoBehaviour
{
	public int cooldown = 1;

	public abstract void DoAction(int lane);
	
}
