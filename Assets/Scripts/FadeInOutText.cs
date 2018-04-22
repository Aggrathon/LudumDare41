using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FadeInOutText : MonoBehaviour {

	public float time = 2;

	Text text;

	private void Awake()
	{
		text = GetComponent<Text>();
	}

	void OnEnable()
	{
		StartCoroutine(Fade());
	}

	IEnumerator Fade()
	{
		float t = 0;
		Color c = text.color;
		while (t < time * 0.5f)
		{
			c.a = 2 * t / time;
			text.color = c;
			t += Time.deltaTime;
			yield return null;
		}
		while (t > 0)
		{
			c.a = 2 * t / time;
			text.color = c;
			t -= Time.deltaTime;
			yield return null;
		}
		gameObject.SetActive(false);
	}
}
