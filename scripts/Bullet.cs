using Godot;
using System;

public partial class Bullet : CharacterBody2D
{
    [Export]
    public float Speed = 700.0f;
    [Export]
    public float VelocityMultiplier = 1.0f;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    private Vector2 direction = new Vector2(1, 0);
    private Vector2 velocity = new Vector2();
	private Vector2 PlayerVelocity = new Vector2();

    public override void _Ready()
    {
        // Set the initial direction based on the rotation of the bullet
        direction = direction.Rotated(Rotation);
		velocity = Speed * direction + PlayerVelocity;
    }

	public override void _PhysicsProcess(double delta)
	{
    	velocity.Y += gravity * (float)delta;

		Velocity = velocity;

		MoveAndSlide();
	}

   public void SetVelocity(Vector2 playerVelocity)
	{
		PlayerVelocity = playerVelocity * VelocityMultiplier;
	}


    private void _on_timer_timeout()
    {
        QueueFree();
    }
}
