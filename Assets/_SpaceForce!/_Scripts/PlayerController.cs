using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float movementSpeed = .2f;
	
	void FixedUpdate () 
	{
		transform.position += Vector3.right * Input.GetAxis("Horizontal") * movementSpeed;
		transform.position += Vector3.up * Input.GetAxis("Vertical") * movementSpeed;

		Vector3 playerForward = transform.forward;
		Vector3 playerRight = Vector3.Cross(Vector3.forward, playerForward);
		transform.position += playerRight * Input.GetAxis("Horizontal") * movementSpeed;
		transform.position += playerForward * Input.GetAxis("Vertical") * movementSpeed;
	}
}
