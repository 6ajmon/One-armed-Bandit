using Godot;
using System;

public partial class Stage : Node2D
{
    public override void _Ready()
    {
        ResetRound();
    }
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void ResetRound()
    {
        foreach (Node child in GetTree().Root.GetChildren())
        {
            if (child is Bullet)
            {
                child.QueueFree();
            }
        }
        GetNode<PlayerSpawnPoints>("PlayerSpawnPoints").SpawnPlayers();
    }
}
