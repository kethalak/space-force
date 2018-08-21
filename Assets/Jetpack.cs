using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class Jetpack : MonoBehaviour
{
	public GameObject[] JetpackFlames;


	private void Awake()
	{
		foreach (var flame in JetpackFlames)
			flame.SetActive(false);
	}

	public void SetJetpackAnimations(bool up, bool down, bool left, bool right)
	{
		JetpackFlames[0].SetActive(up);
		JetpackFlames[1].SetActive(down);

		if (transform.root.localScale.x > 0)
		{
			JetpackFlames[2].SetActive(left);
			JetpackFlames[3].SetActive(right);
		}
		else if (transform.root.localScale.x < 0)
		{
			JetpackFlames[2].SetActive(left);
			JetpackFlames[3].SetActive(right);
		}
	}

	public int[] JetpackFlameStates()
	{
		var states = new int[4];
		states[0] = JetpackFlames[0].activeSelf ? 1 : 0; 
		states[1] = JetpackFlames[1].activeSelf ? 1 : 0;
		states[2] = JetpackFlames[2].activeSelf ? 1 : 0;
		states[3] = JetpackFlames[3].activeSelf ? 1 : 0;
		return states;
	}

	public bool FlameInput(bool active, bool shift)
	{
		return shift && active;
	}
	
	public Vector3 AddJetpackForce(bool up, bool down, bool left, bool right, bool shift)
	{
		var jetpackForce = Vector3.zero;

		if (!shift)
		{
			SetJetpackAnimations(false, false, false, false);
			return Vector3.zero;
		}
        
		SetJetpackAnimations(up, down, left, right);
        
		var jetpackSpeed = 5 * BoltNetwork.frameDeltaTime;
        
		if(up)
			jetpackForce.y += jetpackSpeed;
		if(down)
			jetpackForce.y -= jetpackSpeed;
		if(left)
			jetpackForce.x -= jetpackSpeed;
		if(right)
			jetpackForce.x += jetpackSpeed;		

		return transform.TransformDirection(jetpackForce);    
	}
}


