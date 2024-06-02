using Godot;
using System;

public partial class PlayerMovement : Node2D
{
	private Player player = null;
	public const float Speed = 800.0f;
	public const float JumpVelocity = -600.0f;
	private Vector2 syncPosition = new(0, 0);
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	private int jumpCount = 0;
	public override void _Ready()
	{
		player = GetParent<Player>();
	}

	public override void _PhysicsProcess(double delta)
	{
		if(player.multiplayerSynchronizer.GetMultiplayerAuthority() == player.Multiplayer.GetUniqueId())
		{
			Vector2 velocity = player.Velocity;

			if (!player.IsOnFloor())
				velocity.Y += gravity * (float)delta;

			if (Input.IsActionJustPressed("ui_accept"))
			{
				if (player.IsOnFloor())
				{
					velocity.Y = JumpVelocity;
					jumpCount = 1;
				}
				else if (jumpCount < 1)
				{
					velocity.Y = JumpVelocity;
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
}
