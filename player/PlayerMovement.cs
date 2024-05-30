using Godot;
using System;

public partial class PlayerMovement : Node2D
{
	private Player player = null;
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;
	private Vector2 syncPosition = new(0, 0);
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	private MultiplayerSynchronizer multiplayerSynchronizer = null;
	public override void _Ready()
	{
		player = GetParent<Player>();
		multiplayerSynchronizer = GetNode<MultiplayerSynchronizer>("../MultiplayerSynchronizer");
		multiplayerSynchronizer.SetMultiplayerAuthority(int.Parse(player.Name));
	}

	public override void _PhysicsProcess(double delta)
	{
		if(multiplayerSynchronizer.GetMultiplayerAuthority() == player.Multiplayer.GetUniqueId())
		{
			Vector2 velocity = player.Velocity;

			if (!player.IsOnFloor())
				velocity.Y += gravity * (float)delta;

			if (Input.IsActionJustPressed("ui_accept") && player.IsOnFloor())
				velocity.Y = JumpVelocity;

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
