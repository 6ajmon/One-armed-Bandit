using Godot;
using System;

public partial class Bullet : CharacterBody2D
{
    [Export] public float Speed = 1100.0f;
    [Export] public float Damage = 45.0f;
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    private Vector2 direction = new Vector2(1, 0);
    private Vector2 velocity = new Vector2();
	private Vector2 PlayerVelocity = new Vector2();

    public override void _Ready()
    {
        direction = direction.Rotated(Rotation);
		velocity = Speed * direction + PlayerVelocity;
    }

	public override void _PhysicsProcess(double delta)
	{
    	velocity.Y += gravity * (float)delta;

		Velocity = velocity;

        KinematicCollision2D collision = MoveAndCollide(velocity * (float)delta);
		if (collision != null)
        {
            OnCollision(collision);
        }
	}

    private void _on_timer_timeout()
    {
        QueueFree();
    }

    private void OnCollision(KinematicCollision2D collision)
    {
        QueueFree();
    }

    public void OnBodyEntered(Node body)
    {
        if (body is Player player)
        {
            player.TakeDamage(Damage);
            QueueFree();
        }
    }
}
