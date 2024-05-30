using Godot;
using System;

public partial class PlayerGun : Node2D
{
	private Player player = null;
	private float syncRotation = 0;
	public override void _Ready()
	{
		player = GetParent<Player>();
	}
	public override void _Process(double delta)
	{
		if(player.multiplayerSynchronizer.GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
		{
			Aim();
			if (Input.IsActionJustPressed("shoot"))
			{
				Rpc("Shoot");
			}
			syncRotation = RotationDegrees;
		}
		else {
			RotationDegrees = Mathf.Lerp(RotationDegrees, syncRotation, player.lerpValue);
		}
	}
	private void Aim(){
		LookAt(GetViewport().GetMousePosition());
		var angle = RotationDegrees % 360;
		var gunSprite = GetNode<Sprite2D>("GunSprite");
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
}
