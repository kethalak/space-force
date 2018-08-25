using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
	public static PlanetManager Instance;
	public List<PhysicsObject> PhysicsObjects;

	private void Awake ()
	{
		Instance = this;
		PhysicsObjects = FindObjectsOfType<PhysicsObject>().ToList();
	}
	
	
}
