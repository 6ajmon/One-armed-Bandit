using Godot;
using System;

public partial class PlayerMovement : Node2D
{
	[Export] public const float Speed = 800.0f;
	private Player player = null;
	private Vector2 syncPosition = new(0, 0);
	[Export] private float jumpHeight = 1.0f;
	[Export] private float timeToPeak = 0.25f;
	[Export] private float timeToDescend = 0.25f;
	private float jumpVelocity = 0.0f ;
	private float jumpGravity = 0.0f;
	private float fallGravity = 0.0f;
	private int jumpCount = 0; 
	public override void _Ready()
	{
		player = GetParent<Player>();
		jumpVelocity = (2.0f * jumpHeight / timeToPeak) * -1.0f;
		jumpGravity = (-2.0f * jumpHeight / Mathf.Pow(timeToPeak, 2)) * -1.0f;
		fallGravity = (-2.0f * jumpHeight / Mathf.Pow(timeToDescend, 2)) * -1.0f;
	}

	public override void _PhysicsProcess(double delta)
	{
		if(player.multiplayerSynchronizer.GetMultiplayerAuthority() == player.Multiplayer.GetUniqueId())
		{
			Vector2 velocity = player.Velocity;

			velocity.Y += getGravity() * (float)delta;

			if (Input.IsActionJustPressed("ui_accept"))
			{
				if (player.IsOnFloor())
				{
					velocity.Y = jumpVelocity;
					jumpCount = 1;
				}
				else if (jumpCount < 1)
				{
					velocity.Y = jumpVelocity;
					jumpCount++;
				}
			}

			if (player.IsOnFloor())
				jumpCount = 0;
				

			Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
			if (direction != Vector2.Zero)
			{
				velocity.X = direction.X * Speed;
			}
			else
			{
				velocity.X = Mathf.MoveToward(player.Velocity.X, 0, Speed);
			}

			player.Velocity = velocity;

			player.MoveAndSlide();

			syncPosition = player.GlobalPosition;
		}
		else {
			player.GlobalPosition = player.GlobalPosition.Lerp(syncPosition, player.lerpValue);
		}
	}
	private float getGravity()
	{
		return player.Velocity.Y < 0.0 ? jumpGravity : fallGravity;
	}
}
