using UnityEngine;
using Bolt.AdvancedTutorial;

[BoltGlobalBehaviour("Level2")]
public class PlayerCallbacks : Bolt.GlobalEventListener
{

	public override void SceneLoadLocalDone(string map)
	{
		print("called");
		// this just instantiates our player camera,
		// the Instantiate() method is supplied by the BoltSingletonPrefab<T> class
		OnShipPlayerCamera.Instantiate();
	}
}