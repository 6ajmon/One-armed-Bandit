using Godot;
using System;

public partial class Stage : Node2D
{
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void ResetRound()
    {
        GetNode<PlayerSpawnPoints>("PlayerSpawnPoints").SpawnPlayers();
    }
}
