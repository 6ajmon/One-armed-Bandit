using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] private static float MaxHealth = 100.0f;
	public float Health = MaxHealth;
	[Export] public PackedScene BulletScene;

	[Export] public float lerpValue = .1f;
	public MultiplayerSynchronizer multiplayerSynchronizer = null;

	public override void _Ready()
	{
		multiplayerSynchronizer = GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer");
		multiplayerSynchronizer.SetMultiplayerAuthority(int.Parse(Name));
	}
	public override void _PhysicsProcess(double _delta)
	{
		if(multiplayerSynchronizer.GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
		{
			CollisionLayer = 1;
		}
		else {
			CollisionLayer = 0;
		}
	}
}
