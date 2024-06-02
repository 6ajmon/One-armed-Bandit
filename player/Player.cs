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
	private Godot.Timer spawnProtectionTimer = null;
	private bool spawnProtected = true;

	public override void _Ready()
	{
		multiplayerSynchronizer = GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer");
		multiplayerSynchronizer.SetMultiplayerAuthority(int.Parse(Name));

		healthBar = GetNode<HealthBar>("HealthBar");
		healthBar.MaxValue = MaxHealth;
		healthBar.Value = CurrentHealth;

		GetNode<Label>("NameLabel").Text = Name; //tutaj daje Id gracza zamiast nazwy

		ApplySpawnProtection();
	}
	public override void _PhysicsProcess(double _delta)
	{
		if(multiplayerSynchronizer.GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
		{
		}
		else {
		}
	}
	private void ApplySpawnProtection()
	{
		spawnProtectionTimer = new Godot.Timer();
		spawnProtectionTimer.OneShot = true;
		spawnProtectionTimer.WaitTime = 1.0f;
		AddChild(spawnProtectionTimer);
		spawnProtectionTimer.Start();
		spawnProtectionTimer.Timeout += () => {
			spawnProtected = false;
		};
	}
	public void TakeDamage(float damage)
	{
		if (spawnProtected)
			return;
		CurrentHealth -= damage;
		if (CurrentHealth <= 0)
		{
			Rpc("Die");
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
		GameManager.AddScore(int.Parse(Name));
		ScoreScene scoreScene = GetNode<ScoreScene>("/root/ScoreScene");
		scoreScene.ShowScore();
		QueueFree();
	}
}
