using UnityEngine;
using System.Collections;

public class CameraScroll : MonoBehaviour {

	Player player;
	Transform _pt;
	Transform _t;

	public float InitAcceleration;

	public float PlayerOffset;
	public float Speed;

	void Awake()
	{
		GameState.CurrentPlayState = GameState.PlayState.Init;
		_t = transform;
	}

	void Start()
	{
		player = Player.Current;
		_pt = player.transform;

		speed = 0;
		targetPos.z = _t.position.z;
		targetPos.x = _pt.position.x + PlayerOffset;
		targetPos.y = _pt.position.y;
	}

	float speed;
    Vector3 targetPos;
	Vector3 translatedPos;
	void Update()
	{
		if (GameState.CurrentPlayState == GameState.PlayState.Init)
		{
			speed += Time.fixedDeltaTime * InitAcceleration * GameState.TimeScale;
			translatedPos = _t.position + ((targetPos - _t.position) * speed);

			if (translatedPos.x >= targetPos.x - 0.1f)
			{
				GameState.CurrentPlayState = GameState.PlayState.Playing;
				translatedPos = targetPos;
			}

			_t.position = translatedPos;
		}
		else if (GameState.CurrentPlayState == GameState.PlayState.Playing)
		{
			targetPos.z = _t.position.z;
			targetPos.x = _pt.position.x + PlayerOffset;
			targetPos.y = player.LastGroundedPosition.y;

			if (_t.position.x > targetPos.x)
				targetPos.x = _t.position.x;

			speed = Time.deltaTime * GameState.TimeScale * Speed;
			if (speed > 1)
				speed = 1;

			translatedPos = _t.position + ((targetPos - _t.position) * speed);
			_t.position = translatedPos;
		}
	}
}
