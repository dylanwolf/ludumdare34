using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour {

	Camera cam;
	Transform _t;
	Transform _ct;

	float minCameraX = -29.6f;
	float MaxCameraX = 405;

	float minImgX = 2.17f;
	float maxImgX = -1.9f;

	float lastOrthoSize;
	const float scaleToOrthoSizeRatio = 1.0f / 5.0f;

	void Awake()
	{
		_t = transform;
	}

	void Start () {
		cam = Camera.main;
		_ct = cam.transform;
		minCameraX = _ct.position.x;
		lastOrthoSize = cam.orthographicSize;
	}

	float pct;
	float x;
	Vector3 pos;
	Vector3 scale;
	void Update () {
		if (lastOrthoSize != cam.orthographicSize)
		{
			scale = _t.localScale;
			scale.x = scale.y = (scaleToOrthoSizeRatio * cam.orthographicSize);
			_t.localScale = scale;
			lastOrthoSize = cam.orthographicSize;
		}


		pct = (_ct.position.x - minCameraX) / (MaxCameraX - minCameraX);
		x = (pct * (maxImgX - minImgX)) + minImgX;

		pos = _t.localPosition;
		pos.x = x;
		_t.localPosition = pos;
	}
}
