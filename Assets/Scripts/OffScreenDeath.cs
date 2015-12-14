using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class OffScreenDeath : MonoBehaviour {

	Camera cam;
	BoxCollider2D _c;


	void Awake()
	{
		_c = GetComponent<BoxCollider2D>();
	}
	
	void Start () {
		cam = Camera.main;
		UpdateBoundingBox();
	}

	Vector2 tmp;
	void UpdateBoundingBox()
	{
		tmp = _c.size;
		tmp.x = cam.aspect * cam.orthographicSize * 2;
		tmp.y = cam.orthographicSize * 4;
		_c.size = tmp;

		lastOrthoSize = cam.orthographicSize;
	}

	float lastOrthoSize;

	void Update()
	{
		if (dead)
		{
			if (Input.anyKeyDown)
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
		else
		{
			if (lastOrthoSize != cam.orthographicSize)
				UpdateBoundingBox();
		}
	}

	bool dead = false;
	const string PLAYER_TAG = "Player";
	void OnTriggerExit2D(Collider2D c)
	{
		if (c.tag == PLAYER_TAG && GameState.CurrentPlayState == GameState.PlayState.Playing)
		{
			dead = true;
			GameState.IsAlive = false;
		}
	}

}
