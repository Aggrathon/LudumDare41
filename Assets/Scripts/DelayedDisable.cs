using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedDisable : MonoBehaviour {

	public float time;

	private void OnEnable()
	{
		StartCoroutine(Disable());
	}

	IEnumerator Disable()
	{
		yield return new WaitForSeconds(time);
		gameObject.SetActive(false);
	}
}
