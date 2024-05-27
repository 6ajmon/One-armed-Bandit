using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;
	[Export] public PackedScene BulletScene;
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
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
			Shoot();
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
	private void Shoot()
	{
		Bullet bullet = BulletScene.Instantiate() as Bullet;
		bullet.RotationDegrees = GetNode<Node2D>("GunRotation").RotationDegrees;
		bullet.GlobalPosition = GetNode<Marker2D>("GunRotation/GunSprite/BulletSpawnPoint").GlobalPosition;
	
		// Adjust the bullet's direction based on the player's facing direction
		Vector2 adjustedVelocity = Velocity;
		if (bullet.RotationDegrees > 90 && bullet.RotationDegrees < 270)
		{
			// If the player is facing left, flip the velocity
			adjustedVelocity.X = -adjustedVelocity.X;
		}
	
		bullet.SetVelocity(adjustedVelocity);
	
		GetTree().Root.AddChild(bullet);
	}
}
