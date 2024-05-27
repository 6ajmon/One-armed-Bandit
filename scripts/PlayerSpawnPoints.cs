using Godot;
using System;

public partial class PlayerSpawnPoints : Node2D
{
	[Export] public PackedScene PlayerScene;
	public override void _Ready()
	{
		SpawnPlayer();
	}
	private void SpawnPlayer()
	{
		Player player = PlayerScene.Instantiate() as Player;
		player.GlobalPosition = GetNode<Marker2D>("Player1SpawnPoint").GlobalPosition;
		GetTree().Root.AddChild(player);
	}
}
