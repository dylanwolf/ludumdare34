using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class WinnerArea : MonoBehaviour {

	void Update()
	{
		if (GameState.CurrentPlayState == GameState.PlayState.Paused)
		{
			if (Input.anyKeyDown)
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}

	const string PLAYER_TAG = "Player";
	void OnTriggerStay2D(Collider2D c)
	{
		if (c.tag == PLAYER_TAG && GameState.CurrentPlayState == GameState.PlayState.Playing)
		{
			GameState.HasWon = true;
			GameState.CurrentPlayState = GameState.PlayState.Paused;
		}
	}
}
