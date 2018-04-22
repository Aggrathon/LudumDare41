using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class CameraResizer : MonoBehaviour {

	public float screenWidth = 20;
	public float minHeight = 5;

	private Camera cam;

	float prevSize;

	void Awake()
	{
		cam = GetComponent<Camera>();
		prevSize = cam.orthographicSize;
	}

	void Update()
	{
		float screenHeight = Mathf.Max(minHeight, screenWidth * (float)Screen.height / (float)Screen.width);
		if (screenHeight - prevSize > 0.1 || screenHeight - prevSize < -0.1)
		{
			cam.orthographicSize = screenHeight;
			prevSize = screenHeight;

		}
	}
}
