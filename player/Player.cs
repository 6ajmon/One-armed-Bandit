using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] private static float MaxHealth = 100.0f;
	public float Health = MaxHealth;
	[Export] public PackedScene BulletScene;
	private float syncRotation = 0;
	[Export] public float lerpValue = .1f;

	public override void _Ready()
	{
		GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(int.Parse(Name));
	}
	public override void _PhysicsProcess(double delta)
	{
		if(GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
		{
			Aim();

			if (Input.IsActionJustPressed("shoot"))
			{
				Rpc("Shoot");
			}
			syncRotation = GetNode<Node2D>("GunRotation").RotationDegrees;
		}
		else {
			GetNode<Node2D>("GunRotation").RotationDegrees = Mathf.Lerp(GetNode<Node2D>("GunRotation").RotationDegrees, syncRotation, lerpValue);
		}
	}
	private void Aim(){
		GetNode<Node2D>("GunRotation").LookAt(GetViewport().GetMousePosition());
		var angle = GetNode<Node2D>("GunRotation").RotationDegrees % 360;
		if (angle < 0) angle += 360;
		if (angle > 360) angle -= 360; 
		if (angle > 90 && angle < 270) 
			GetNode<Sprite2D>("GunRotation/GunSprite").FlipV = true;
		else 
			GetNode<Sprite2D>("GunRotation/GunSprite").FlipV = false;
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void Shoot()
	{
		Bullet bullet = BulletScene.Instantiate() as Bullet;
		bullet.RotationDegrees = GetNode<Node2D>("GunRotation").RotationDegrees;
		bullet.GlobalPosition = GetNode<Marker2D>("GunRotation/GunSprite/BulletSpawnPoint").GlobalPosition;
	
		bullet.SetVelocity(Velocity);
	
		GetTree().Root.AddChild(bullet, true);
	}
}
