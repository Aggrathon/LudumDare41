using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour, IHelp {

	public SpriteRenderer sprite;
	public GameObject deathFX;
	int cd;

	public AAbility ability;

	[SerializeField][TextArea] string _description;
	public string description { get { return _description; } }
	public string title { get { return name; } }

	void Start () {
		if (sprite == null)
			sprite = GetComponent<SpriteRenderer>();
		GetComponentInParent<HeroController>().SpawnHero(this);
		cd = -1;
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
		return cd < 0;
	}

	public void StartTurn()
	{
		cd--;
	}
}

public abstract class AAbility : MonoBehaviour
{
	public int cooldown = 1;

	public abstract void DoAction(int lane);
	
}
