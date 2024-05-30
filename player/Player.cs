using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] private static float MaxHealth = 100.0f;
	public float CurrentHealth = MaxHealth;
	[Export] public PackedScene BulletScene;

	[Export] public float lerpValue = .1f;
	public MultiplayerSynchronizer multiplayerSynchronizer = null;
	private HealthBar healthBar = null;

	public override void _Ready()
	{
		multiplayerSynchronizer = GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer");
		multiplayerSynchronizer.SetMultiplayerAuthority(int.Parse(Name));
		healthBar = GetNode<HealthBar>("HealthBar");
		healthBar.MaxValue = MaxHealth;
		healthBar.Value = CurrentHealth;
	}
	public override void _PhysicsProcess(double _delta)
	{
		if(multiplayerSynchronizer.GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
		{
		}
		else {
		}
	}
	public void TakeDamage(float damage)
	{
		CurrentHealth -= damage;
		if (CurrentHealth <= 0)
		{
			Die();
		}
		healthBar.SetHealth(CurrentHealth);
	}
	private void Die()
	{
		QueueFree();
	}
}
