using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public static Player Current;

	public float RunSpeed;
	public float Gravity;
	public float JumpTime;
	public float JumpHeight;
	public float FallTime;
	public float CurrentScale = 1.0f;
	public float MinScale;
	public float MaxScale;
	public float ScaleIncreasePerSecond;

	tk2dSpriteAnimator anim;

	bool isCrouched = false;

	[System.NonSerialized]
	public Vector3 LastGroundedPosition;

	public Transform[] HeadPoints;
	public Transform[] CrouchedHeadPoints;
	public Transform[] FeetPoints;

	[System.NonSerialized]
	public PlayerState CurrentState = PlayerState.Standing;

	public enum PlayerState
	{
		Standing,
		Falling,
		Jumping
	}

	const string COLLISION_LAYER = "Collision";
	int collisionLayer;
	Rigidbody2D _r;
	Transform _t;

	void Awake()
	{
		Current = this;
		_r = GetComponent<Rigidbody2D>();
		_t = transform;
		anim = GetComponent<tk2dSpriteAnimator>();
	}

	void Start()
	{
		collisionLayer = LayerMask.GetMask(COLLISION_LAYER);
		CurrentState = PlayerState.Standing;
		LastGroundedPosition = _t.position;
		CurrentScale = MinScale;
	}

	Vector2 velocity;
	Vector3 scale;
	Vector3 pos;
	void FixedUpdate()
	{
		if (GameState.CurrentPlayState == GameState.PlayState.Playing)
		{
			UpdatePlayerState();
			velocity.x = RunSpeed * GameState.TimeScale * CurrentScale;
			_r.velocity = velocity;

			if (CurrentScale < MaxScale)
			{
				CurrentScale += (ScaleIncreasePerSecond * GameState.TimeScale);
				if (CurrentScale > MaxScale)
					CurrentScale = MaxScale;

				scale = _t.localScale;
				scale.x = CurrentScale;
				scale.y = CurrentScale;
				_t.localScale = scale;
			}

		}
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		foreach (var t in HeadPoints)
		{
			Gizmos.DrawLine(t.position, t.position + (RAYCAST_DISTANCE * new Vector3(Vector2.up.x, Vector2.up.y, 0)));
		}

		Gizmos.color = Color.red;
		foreach (var t in FeetPoints)
		{
			Gizmos.DrawLine(t.position, t.position + (RAYCAST_DISTANCE * -new Vector3(Vector2.up.x, Vector2.up.y, 0)));
		}
	}


	Vector2 tmpPos;
	RaycastHit2D hit;
	const float RAYCAST_DISTANCE = 0.1001f;
	bool foundHit = false;
	void UpdatePlayerState()
	{
		// If jumping, try going to falling
		if (CurrentState == PlayerState.Jumping)
		{
			foreach (var t in isCrouched ? CrouchedHeadPoints : HeadPoints)
			{
				tmpPos.x = t.position.x;
				tmpPos.y = t.position.y;

				hit = Physics2D.Raycast(tmpPos, Vector2.up, RAYCAST_DISTANCE, collisionLayer);
				if (hit.collider != null)
				{
					StartCoroutine(COROUTINE_DOFALLING);
					break;
				}
			}
		}
		// If standing, try going to falling
		else if (CurrentState == PlayerState.Standing)
		{
			foundHit = false;
			foreach (var t in FeetPoints)
			{
				tmpPos.x = t.position.x;
				tmpPos.y = t.position.y;

				hit = Physics2D.Raycast(tmpPos, -Vector2.up, RAYCAST_DISTANCE, collisionLayer);
				if (hit.collider != null)
				{
					foundHit = true;
					break;
				}
			}

			if (!foundHit)
			{
				StartCoroutine(COROUTINE_DOFALLING);
			}
		}
		// If falling, try going to standing
		else if (CurrentState == PlayerState.Falling)
		{
			foreach (var t in FeetPoints)
			{
				tmpPos.x = t.position.x;
				tmpPos.y = t.position.y;

				hit = Physics2D.Raycast(tmpPos, -Vector2.up, RAYCAST_DISTANCE, collisionLayer);
				if (hit.collider != null)
				{
					CurrentState = PlayerState.Standing;
					LastGroundedPosition = _t.position;
					velocity.y = 0;
					break;
				}
			}
		}
	}

	const string INPUT_UP = "Up";
	const string INPUT_DOWN = "Down";
	const string COROUTINE_DOJUMPING = "DoJumping";
	const string COROUTINE_DOFALLING = "DoFalling";

	float pct;
	IEnumerator DoJumping()
	{
		CurrentState = PlayerState.Jumping;

		for(float t = 0; t < JumpTime; t += Time.fixedDeltaTime)
		{
			if (GameState.CurrentPlayState != GameState.PlayState.Playing)
			{
				t -= Time.fixedDeltaTime;
				velocity.y = 0;
				yield return new WaitForFixedUpdate();
			}

			if (!Input.GetButton(INPUT_UP))
				break;

			if (CurrentState == PlayerState.Falling)
			{
				break;
			}
			else if (CurrentState == PlayerState.Standing)
			{
				velocity.y = 0;
				break;
			}

			pct = t / JumpTime;
			velocity.y = JumpHeight * CurrentScale * (Mathf.PI * Mathf.Cos(Mathf.PI * pct / 2)) / 2;
			yield return new WaitForFixedUpdate();
		}

		if (CurrentState != PlayerState.Standing)
			StartCoroutine(COROUTINE_DOFALLING);
	}

	IEnumerator DoFalling()
	{
		CurrentState = PlayerState.Falling;

		float t = 0;
		while (CurrentState == PlayerState.Falling)
		{
			if (GameState.CurrentPlayState != GameState.PlayState.Playing)
			{
				yield return new WaitForFixedUpdate();
			}

			if (t < FallTime)
				t += Time.fixedDeltaTime;
			else if (t > FallTime)
				t = FallTime;

			pct = 1 - (t / FallTime);
			velocity.y = Gravity * -JumpHeight * CurrentScale * (Mathf.PI * Mathf.Cos(Mathf.PI * pct / 2)) / 2;

			yield return new WaitForFixedUpdate();
		}
	}

	bool wasCrouched = true;
	const string ANIM_CROUCH = "Crouch";
	const string ANIM_STAND = "Stand";

	// Update is called once per frame
	void Update () {

		// Update velocity
		if (GameState.CurrentPlayState == GameState.PlayState.Playing)
		{
			// Test for jump
			if (Input.GetButton(INPUT_UP) && CurrentState == PlayerState.Standing)
			{
				StartCoroutine(COROUTINE_DOJUMPING);
			}

			// Test for crouch
			isCrouched = Input.GetButton(INPUT_DOWN);
			if (isCrouched != wasCrouched)
			{
				anim.Play(isCrouched ? ANIM_CROUCH : ANIM_STAND);
			}
			wasCrouched = isCrouched;
		}
		else
		{
			velocity.x = 0;
			velocity.y = 0;
		}

	}
}
