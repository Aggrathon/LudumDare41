using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomSound : MonoBehaviour {

	public AudioClip[] sounds;
	public AudioSource source;

	private void Awake()
	{
		source = GetComponent<AudioSource>();
	}

	private void OnEnable()
	{
		source.pitch = 0.75f + Random.value * 0.4f;
		source.PlayOneShot(sounds[Random.Range(0, sounds.Length)]);
	}
}
