using UnityEngine;
using System.Collections;

public static class GameState {

	public static PlayState CurrentPlayState = PlayState.Playing;
	public static float TimeScale = 1.0f;

	public enum PlayState
	{
		Init,
		Playing,
		Paused
	}
}
