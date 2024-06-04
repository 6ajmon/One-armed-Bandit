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
        if (Multiplayer.IsServer())
        {
            RemoveAllBullets();
            Rpc(nameof(RemoveAllBullets));
        }
        
        GetNode<PlayerSpawnPoints>("PlayerSpawnPoints").SpawnPlayers();
    }
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void RemoveAllBullets()
    {
        foreach (Node child in GetTree().Root.GetChildren())
        {
            if (child is Bullet)
            {
                child.QueueFree();
            }
        }
    }
}
