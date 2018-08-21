using UnityEngine;
   
public class PlayerObject
{
	public BoltEntity character;
	public BoltConnection connection;
	
	public bool IsServer => connection == null;

	public bool IsClient => connection != null;

	public void Spawn()
	{
		if (!character)
		{
			character = BoltNetwork.Instantiate(BoltPrefabs.SpaceCadet);
			PlanetManager.Instance.PhysicsObjects.Add(character.gameObject.GetComponent<PhysicsObject>());
			if (IsServer)
			{
				character.TakeControl();
			} 
			else 
			{
				character.AssignControl(connection);
			}
		}

		// teleport entity to a spawn position
		character.transform.position = new Vector3(0,0,0);
	}
}