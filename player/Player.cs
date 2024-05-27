using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;
	[Export] public PackedScene BulletScene;
	private Vector2 syncPosition = new(0, 0);
	private float syncRotation = 0;
	[Export] private float lerpValue = .1f;
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _Ready()
	{
		GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(int.Parse(Name));
	}
	public override void _PhysicsProcess(double delta)
	{
		if(GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
		{
			Vector2 velocity = Velocity;

			if (!IsOnFloor())
				velocity.Y += gravity * (float)delta;

			if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
				velocity.Y = JumpVelocity;

			Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
			if (direction != Vector2.Zero)
			{
				velocity.X = direction.X * Speed;
			}
			else
			{
				velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			}

			Velocity = velocity;

			MoveAndSlide();

			Aim();

			if (Input.IsActionJustPressed("shoot"))
			{
				Rpc("Shoot");
			}
			syncPosition = GlobalPosition;
			syncRotation = GetNode<Node2D>("GunRotation").RotationDegrees;
		}
		else {
			GlobalPosition = GlobalPosition.Lerp(syncPosition, lerpValue);
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
