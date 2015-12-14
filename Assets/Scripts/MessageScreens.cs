using UnityEngine;
using System.Collections;

public class MessageScreens : MonoBehaviour {

	public SpriteRenderer GameOverScreen;
	public SpriteRenderer WinnerScreen;

	Transform _got;
	Transform _wst;

	Vector3 scale;

	float scaleToOrthosize = 1.0f/5.0f;

	Camera cam;

	void Start()
	{
		cam = Camera.main;
		GameState.IsAlive = true;
		GameState.HasWon = false;
		_got = GameOverScreen.transform;
		_wst = WinnerScreen.transform;
	}

	// Update is called once per frame
	void Update () {
		GameOverScreen.enabled = !GameState.IsAlive;
		WinnerScreen.enabled = GameState.HasWon;

		if (GameOverScreen.enabled)
		{
			scale = _got.localScale;
			scale.x = scale.y = (cam.orthographicSize * scaleToOrthosize);
			_got.localScale = scale;
		}

		if (WinnerScreen.enabled)
		{
			scale = _wst.localScale;
			scale.x = scale.y = (cam.orthographicSize * scaleToOrthosize);
			_wst.localScale = scale;
		}
	}
}
