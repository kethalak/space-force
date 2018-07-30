using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour 
{
    public struct State
    {
        public Vector2 position;
        public Vector2 velocity;
        public bool isGrounded;
        public int jumpFrames;
    }

    private State _state;
    private CharacterController2D _cc;
    
    [SerializeField]
    float skinWidth = 0.08f;

    [SerializeField]
    float gravityForce = -9.81f;

    [SerializeField]
    float jumpForce = +40f;

    [SerializeField]
    int jumpTotalFrames = 30;

    [SerializeField]
    float movingSpeed = 4f;

    [SerializeField]
    float maxVelocity = 32f;

    [SerializeField]
    Vector3 drag = new Vector3 (1f, 0f, 1f);

    [SerializeField]
    LayerMask layerMask;

	private Vector3 sphere
	{
		get
		{
			Vector3 p;

			p = transform.position;
			p.y += _cc.Radius;
			p.y -= (skinWidth * 2);

			return p;
		}
	}

	private Vector3 waist 
	{
		get 
		{
			Vector3 p;

			p = transform.position;
			p.y += _cc.Height / 2;

			return p;
		}
	}

	public bool jumpStartedThisFrame {
		get {
			return _state.jumpFrames == (jumpTotalFrames - 1);
		}
	}

	void Awake ()
	{
		_cc = GetComponent<CharacterController2D> ();
		_state = new State ();
		_state.position = transform.localPosition;
	}

	public void SetState (Vector2 position, Vector2 velocity, bool isGrounded, int jumpFrames)
	{
		// assign new state
		_state.position = position;
		_state.velocity = velocity;
		_state.jumpFrames = jumpFrames;
		_state.isGrounded = isGrounded;

		// assign local position
		transform.localPosition = _state.position;
	}

	void Move (Vector2 velocity)
	{
		bool isGrounded = false;
		
		if(isGrounded)
			_cc.Move (velocity * Time.deltaTime, false);

		if (isGrounded && !_state.isGrounded) 
			_state.velocity = new Vector2 ();

		_state.isGrounded = isGrounded;
		_state.position = transform.localPosition;
	}

	public State Move (bool forward, bool backward, bool left, bool right, bool jump, float yaw)
	{
		var moving = false;
		var movingDir = Vector2.zero;

		if (left ^ right) {
			movingDir.x = right ? +1 : -1;
		}

		if (_state.isGrounded) {
			if (jump && _state.jumpFrames == 0) {
				_state.jumpFrames = (byte)jumpTotalFrames;
				_state.velocity += movingDir * movingSpeed;
			}

			if (moving && _state.jumpFrames == 0) {
				Move (movingDir * movingSpeed);
			}
		} else {
			_state.velocity.y += gravityForce * BoltNetwork.frameDeltaTime;
		}

		if (_state.jumpFrames > 0) {
			// calculate force
			float force;
			force = (float)_state.jumpFrames / (float)jumpTotalFrames;
			force = jumpForce * force;

			Move (new Vector3 (0, force, 0));
		}

		// decrease jump frames
		_state.jumpFrames = Mathf.Max (0, _state.jumpFrames - 1);

		// clamp velocity
		_state.velocity = Vector3.ClampMagnitude (_state.velocity, maxVelocity);

		// apply drag
		_state.velocity.x = ApplyDrag (_state.velocity.x, drag.x);
		_state.velocity.y = ApplyDrag (_state.velocity.y, drag.y);


		// this might seem weird, but it actually gets around a ton of issues - we basically apply 
		// gravity on the Y axis on every frame to simulate instant gravity if you step over a ledge
		_state.velocity.y = Mathf.Min (_state.velocity.y, gravityForce);

		// apply movement
		Move (_state.velocity);

		// set local rotation
		transform.localRotation = Quaternion.Euler (0, yaw, 0);

		// detect tunneling
		DetectTunneling ();

		// update position
		_state.position = transform.localPosition;

		// done
		return _state;
	}

	float ApplyDrag (float value, float drag)
	{
		if (value < 0) {
			return Mathf.Min (value + (drag * BoltNetwork.frameDeltaTime), 0f);
		} else if (value > 0) {
			return Mathf.Max (value - (drag * BoltNetwork.frameDeltaTime), 0f);
		}

		return value;
	}

	void DetectTunneling ()
	{
		RaycastHit hit;

		if (Physics.Raycast (waist, Vector3.down, out hit, _cc.Height / 2, layerMask)) {
			transform.position = hit.point;
		}
	}

	void OnDrawGizmos ()
	{
		if (Application.isPlaying) {
			Gizmos.color = _state.isGrounded ? Color.green : Color.red;
			Gizmos.DrawWireSphere (sphere, _cc.Radius);

			Gizmos.color = Color.magenta;
			Gizmos.DrawLine (waist, waist + new Vector3 (0, -(_cc.Height / 2f), 0));
		}
	}
}

