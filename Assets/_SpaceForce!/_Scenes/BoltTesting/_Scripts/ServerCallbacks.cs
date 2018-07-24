using UnityEngine;

[BoltGlobalBehaviour(BoltNetworkModes.Host, "Level2")]
public class ServerCallbacks : Bolt.GlobalEventListener
{
    private void Awake()
    {
        PlayerObjectRegistry.CreateServerPlayer();
    }

    public override void Connected(BoltConnection connection)
    {
        PlayerObjectRegistry.CreateClientPlayer(connection);
    }

    public override void SceneLoadLocalDone(string map) 
    {
        PlayerObjectRegistry.ServerPlayer.Spawn();
    }

    public override void SceneLoadRemoteDone(BoltConnection connection) 
    {
        PlayerObjectRegistry.GetPlayer(connection).Spawn();
    }
}