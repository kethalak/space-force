using UnityEngine;

[BoltGlobalBehaviour("Level2")]
public class PlayerCallbacks : Bolt.GlobalEventListener
{

	public override void SceneLoadLocalDone(string map)
	{
		// this just instantiates our player camera,
		// the Instantiate() method is supplied by the BoltSingletonPrefab<T> class
		OnShipPlayerCamera.Instantiate();
	}

	public override void ControlOfEntityGained(BoltEntity entity)
	{
		// this tells the player camera to look at the entity we are controlling
		OnShipPlayerCamera.instance.SetTarget(entity);
	}
}
