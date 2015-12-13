using UnityEngine;
using System.Collections;

public class CameraScroll : MonoBehaviour {

	Camera cam;
	Player player;
	Transform _pt;
	Transform _t;
	Rigidbody2D _pr;
	Rigidbody2D _r;

	bool started = false;
	float defaultSize;

	public float InitAcceleration;

	public float FailSpeed;
	public float RecoverySpeed;
	public float ZoomSpeed;
	public float PlayerOffset;
	public float Speed;

	void Awake()
	{
		GameState.CurrentPlayState = GameState.PlayState.Init;
		_t = transform;
		_r = GetComponent<Rigidbody2D>();
	}

	void Start()
	{
		cam = Camera.main;
		defaultSize = cam.orthographicSize = 5;
		player = Player.Current;
		_pt = player.transform;
		_pr = player.GetComponent<Rigidbody2D>();

		speed = 0;
		targetPos.z = _t.position.z;
		targetPos.x = _pt.position.x + PlayerOffset;
		targetPos.y = _pt.position.y;
	}

	float extraSize;
	void SetTargetSize()
	{
		if (Player.Current.CurrentScale >= 1f)
		{
			targetSize = 17;
		}
		else if (Player.Current.CurrentScale > 0.8f)
		{
			targetSize = 15;
		}
		else if (Player.Current.CurrentScale > 0.5f)
		{
			targetSize = 12;
		}
		if (Player.Current.CurrentScale > 0.3f)
		{
			targetSize = 9;
		}
		else if (Player.Current.CurrentScale > 0.15f)
		{
			targetSize = 7;
		}
		else
		{
			targetSize = 5;
		}
	}

	float speed;
    Vector3 targetPos;
	Vector3 translatedPos;
	float targetSize;
	float scale;
	float newSize;
	float scaleSpeed;

	float bufferX;
	float expectedPlayerVelocity;

	void Update()
	{
		if (GameState.CurrentPlayState == GameState.PlayState.Init)
		{
			if (started)
			{
				speed += Time.fixedDeltaTime * InitAcceleration * GameState.TimeScale;
				translatedPos = _t.position + ((targetPos - _t.position) * speed);

				if (translatedPos.x >= targetPos.x - 0.1f)
				{
					GameState.CurrentPlayState = GameState.PlayState.Playing;
					translatedPos = targetPos;

					bufferX = PlayerOffset;
				}

				_t.position = translatedPos;
			}
			else
			{
				if (Input.anyKeyDown)
					started = true;
			}
		}
		else if (GameState.CurrentPlayState == GameState.PlayState.Playing)
		{
			SetTargetSize();
			scale = targetSize / defaultSize;

			// Scale camera zoom
			if (cam.orthographicSize < targetSize)
			{
				scaleSpeed = Time.deltaTime * GameState.TimeScale * ZoomSpeed;
				if (scaleSpeed > 1)
					scaleSpeed = 1;
				newSize = cam.orthographicSize + ((targetSize - cam.orthographicSize) * scaleSpeed);
				cam.orthographicSize = newSize;
			}

			// See if player has been detained
			expectedPlayerVelocity = (player.CurrentScale * GameState.TimeScale * player.RunSpeed) - 0.1f;

			if (_pr.velocity.x < expectedPlayerVelocity)
			{
				bufferX += (expectedPlayerVelocity - _pr.velocity.x) * Time.deltaTime * GameState.TimeScale * FailSpeed;
			}
			else
			{
				bufferX -= (RecoverySpeed * Time.deltaTime * GameState.TimeScale);
			}

			if (bufferX < PlayerOffset)
				bufferX = PlayerOffset;

			// Lock Y position
			targetPos = _t.position;
			targetPos.x = _pt.position.x + bufferX;
			targetPos.y = player.LastGroundedPosition.y;

			speed = Time.deltaTime * GameState.TimeScale * Speed * scale;
			if (speed > 1)
				speed = 1;

			translatedPos = _t.position + ((targetPos - _t.position) * speed);
			_t.position = translatedPos;
		}
	}
}
