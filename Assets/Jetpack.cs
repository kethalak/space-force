using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jetpack : MonoBehaviour
{
	[SerializeField] private GameObject _upFlame;
	[SerializeField] private GameObject _downFlame;
	[SerializeField] private GameObject _sideFlame;

	private void Awake()
	{
		_upFlame.SetActive(false);
		_downFlame.SetActive(false);
		_sideFlame.SetActive(false);
	}

	public Vector3 HandleInput(bool up, bool down, bool left, bool right, bool jump, bool shift)
	{
		
		SetJetpackAnimation(up, down, left, right, shift);

		if (!shift)
			return Vector3.zero;
		
		var jetpackForce = Vector3.zero;	
		var jetpackSpeed = 5 * BoltNetwork.frameDeltaTime;
		
		if (up)
			jetpackForce.y += jetpackSpeed;
		if (down)
			jetpackForce.y -= jetpackSpeed;
		if (left)
			jetpackForce.x -= jetpackSpeed;
		if (right)
			jetpackForce.x += jetpackSpeed;

		return jetpackForce;
	}

	private void SetJetpackAnimation(bool up, bool down, bool left, bool right, bool shift)
	{
		_upFlame.SetActive(up);
		_downFlame.SetActive(down);
		_sideFlame.SetActive(left || right);

		if (shift) return;
		
		_upFlame.SetActive(false);
		_downFlame.SetActive(false);
		_sideFlame.SetActive(false);
	}
}


