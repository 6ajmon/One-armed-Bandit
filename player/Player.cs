using Godot;
using System;
using System.Threading;

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

		GetNode<Label>("NameLabel").Text = Name; //tutaj daje Id gracza zamiast nazwy
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
			Rpc("Die");
			GameManager.AddScore(int.Parse(Name));
		}
		if (GetNodeOrNull<HealthBar>("HealthBar") != null)
			healthBar.Rpc("SetHealth", CurrentHealth);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void Die()
	{
		Godot.Timer timer = new();
		timer.WaitTime = 0.1f;
		timer.OneShot = true;
		timer.Timeout += ActuallyDie;
		AddChild(timer);
		timer.Start();
	}

	private void ActuallyDie()
	{
		ScoreScene scoreScene = GetNode<ScoreScene>("/root/ScoreScene");
		scoreScene.ShowScore();
		QueueFree();
	}
}
