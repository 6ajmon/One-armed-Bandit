using Godot;
using System;

public partial class PlayerGun : Node2D
{
	private Player player = null;
	private float syncRotation = 0;
	private bool canShoot = true;
	private bool isFlipped = false;
	private Sprite2D gunSprite = null;
	public override void _Ready()
	{
		player = GetParent<Player>();
		gunSprite = GetNode<Sprite2D>("GunSprite");
	}
	public override void _Process(double delta)
	{
		var timer = GetNode<Timer>("ShootingCooldown");
		if(player.multiplayerSynchronizer.GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
		{
			Aim();
			if (Input.IsActionJustPressed("shoot"))
			{
				if (canShoot){
					canShoot = false;
					timer.Start();
					Rpc("Shoot");
				}
				else{
					GD.Print("Can't shoot yet " + timer.TimeLeft);
				}
			}
			syncRotation = RotationDegrees;
			isFlipped = gunSprite.FlipV;
		}
		else {
			RotationDegrees = Mathf.Lerp(RotationDegrees, syncRotation, player.lerpValue);
			gunSprite.FlipV = isFlipped;
		}
	}
	private void Aim(){
		LookAt(GetViewport().GetMousePosition());
		var angle = RotationDegrees % 360;
		if (angle < 0) angle += 360;
		if (angle > 360) angle -= 360; 
		if (angle > 90 && angle < 270) 
			gunSprite.FlipV = true;
		else 
			gunSprite.FlipV = false;
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void Shoot()
	{
		Bullet bullet = player.BulletScene.Instantiate() as Bullet;
		bullet.RotationDegrees = RotationDegrees;
		var spawnPoint = GetNode<Marker2D>("GunSprite/BulletSpawnPoint");
		bullet.GlobalPosition = spawnPoint.GlobalPosition;
	
		bullet.SetVelocity(player.Velocity);
	
		GetTree().Root.AddChild(bullet, true);
	}
	private void OnShootingCooldownTimeout()
	{
		canShoot = true;
	}
}
