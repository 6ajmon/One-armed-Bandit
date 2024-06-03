using Godot;
using System;

public partial class PlayerGun : Node2D
{
	[Export] public int magazineSize = 3;
	private Player player = null;
	private float syncRotation = 0;
	private bool canShoot = false;
	private int remainingBullets = 0;
	private bool isFlipped = false;
	private Sprite2D gunSprite = null;
	private bool reloading = false;
	private AmmoBar ammoBar = null;
	private bool isMoved = false;
	Vector2 ammoBarPosition = new();
	public override void _Ready()
	{
		player = GetParent<Player>();
		gunSprite = GetNode<Sprite2D>("GunSprite");
		remainingBullets = magazineSize;
		ammoBar = GetNode<AmmoBar>("AmmoBar");
		ammoBar.SetAmmo(remainingBullets);
		ammoBarPosition = ammoBar.Position;
	}
	public override void _Process(double delta)
	{
		var shootingCooldown = GetNode<Timer>("ShootingCooldown");
		var reload = GetNode<Timer>("Reload");
		if(player.multiplayerSynchronizer.GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
		{
			Aim();
			if (remainingBullets == 0 && !reloading)
			{
				reloading = true;
				reload.Start();
			}
			if (Input.IsActionJustPressed("shoot"))
			{
				if (canShoot && remainingBullets > 0){
					remainingBullets--;
					ammoBar.SetAmmo(remainingBullets);
					canShoot = false;
					shootingCooldown.Start();
					Rpc("Shoot");
				}
				else{
					GD.Print("Can't shoot yet " + shootingCooldown.TimeLeft + " " + remainingBullets);
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
		var sprite = player.GetNode<Sprite2D>("PlayerSprite");
		
		if (angle < 0) angle += 360;
		if (angle > 360) angle -= 360; 
		if (angle > 90 && angle < 270) {
			gunSprite.FlipV = true;
			sprite.FlipH = true;
			if (!isMoved)
				ammoBar.Position = ammoBarPosition + new Vector2(0, 20);
				isMoved = true;
		}
		else {
			gunSprite.FlipV = false;
			sprite.FlipH = false;
			ammoBar.Position = ammoBarPosition;
			isMoved = false;
		}
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void Shoot()
	{
		Bullet bullet = player.BulletScene.Instantiate() as Bullet;
		bullet.RotationDegrees = RotationDegrees;
		var spawnPoint = GetNode<Marker2D>("GunSprite/BulletSpawnPoint");
		bullet.GlobalPosition = spawnPoint.GlobalPosition;
	
		GetTree().Root.AddChild(bullet, true);
	}
	private void OnShootingCooldownTimeout()
	{
		canShoot = true;
	}
	private void OnReloadTimeout()
	{
		GD.Print("Reloaded");
		remainingBullets = magazineSize;
		ammoBar.SetAmmo(remainingBullets);
		reloading = false;
	}
}
