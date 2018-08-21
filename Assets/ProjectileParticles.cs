using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileParticles : MonoBehaviour
{
	[SerializeField] private ParticleSystem[] _collisionFX;

	private List<ParticleCollisionEvent> _collisionEvents;
	private ParticleSystem _projectiles;

	private void Awake()
	{
		_projectiles = GetComponent<ParticleSystem>();
		_collisionEvents = new List<ParticleCollisionEvent>();		
		
	}
	
	private void OnParticleCollision(GameObject other)
	{
		_projectiles.GetCollisionEvents(other, _collisionEvents);

		foreach (var c in _collisionEvents)
		{
			if (c.colliderComponent != null && c.colliderComponent.transform.root == this.transform.root) return;
			print(c.colliderComponent.transform.name);
			EmitAtLocation(c);
		}
	}

	private void EmitAtLocation(ParticleCollisionEvent particleCollisionEvent)
	{
		foreach (var fx in _collisionFX)
		{
			fx.transform.position = particleCollisionEvent.intersection;
			fx.transform.rotation = Quaternion.LookRotation(particleCollisionEvent.normal);
			fx.Play();
		}
	}
}
