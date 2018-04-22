﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectionMarkers : MonoBehaviour {

	public static SelectionMarkers instance { get; protected set; }
	public Button template;

	Camera cam;
	List<Button> markers;
	int activeIndex;

	void Awake()
	{
		activeIndex = 0;
		markers = new List<Button>();
		cam = Camera.main;
		instance = this;
		template.onClick.RemoveAllListeners();
		template.onClick.AddListener(Reset);
	}

	void OnClick(int num)
	{
		Reset();
	}

	public void Reset()
	{
		for (int i = 0; i < activeIndex; i++)
		{
			markers[i].gameObject.SetActive(false);
		}
		activeIndex = 0;
	}

	public void AddMarker(Vector3 pos, UnityAction callback)
	{
		Button button;
		if (activeIndex == markers.Count)
		{
			button = Instantiate<Button>(template, transform);
			markers.Add(button);
		}
		else
		{
			button = markers[activeIndex];
		}
		activeIndex++;
		button.gameObject.SetActive(true);
		button.transform.position = pos;
		button.onClick.RemoveAllListeners();
		button.onClick.AddListener(Reset);
		button.onClick.AddListener(callback);
	}
}