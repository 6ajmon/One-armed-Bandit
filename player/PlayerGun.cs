using Godot;
using System;

public partial class PlayerGun : Node2D
{
	[Export] public int magazineSize = 3;
	private Player player = null;
	private float syncRotation = 0;
	private bool canShoot = false;
	private int remainingBullets = 0;
	private bool isGunSpriteFlipped = false;
	private bool isPlayerSpriteFlipped = false;
	private Sprite2D gunSprite = null;
	private bool reloading = false;
	private AmmoBar ammoBar = null;
	private bool isAmmoBarMoved = false;
	private Vector2 ammoBarPosition = new();
	private Sprite2D playerSprite = null;
	public override void _Ready()
	{
		player = GetParent<Player>();
		gunSprite = GetNode<Sprite2D>("GunSprite");
		remainingBullets = magazineSize;
		ammoBar = GetNode<AmmoBar>("AmmoBar");
		ammoBar.SetAmmo(remainingBullets);
		ammoBarPosition = ammoBar.Position;
		playerSprite = player.GetNode<Sprite2D>("PlayerSprite");
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
				ammoBar.StartReloading();
			}
			if (Input.IsActionJustPressed("shoot"))
			{
				if (canShoot && remainingBullets > 0){
					remainingBullets--;
					ammoBar.SetAmmo(remainingBullets);
					canShoot = false;
					shootingCooldown.Start();
					Rpc(nameof(Shoot));
				}
			}
			syncRotation = RotationDegrees;
			isGunSpriteFlipped = gunSprite.FlipV;
			isPlayerSpriteFlipped = playerSprite.FlipH;
		}
		else {
			RotationDegrees = Mathf.Lerp(RotationDegrees, syncRotation, player.lerpValue);
			gunSprite.FlipV = isGunSpriteFlipped;
			playerSprite.FlipH = isPlayerSpriteFlipped;
		}
	}
	private void Aim(){
		LookAt(GetViewport().GetMousePosition());
		var angle = RotationDegrees % 360;
		
		if (angle < 0) angle += 360;
		if (angle > 360) angle -= 360; 
		if (angle > 90 && angle < 270) {
			gunSprite.FlipV = true;
			playerSprite.FlipH = true;
			if (!isAmmoBarMoved)
				ammoBar.Position = ammoBarPosition + new Vector2(0, 20);
				isAmmoBarMoved = true;
		}
		else {
			gunSprite.FlipV = false;
			playerSprite.FlipH = false;
			ammoBar.Position = ammoBarPosition;
			isAmmoBarMoved = false;
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
		remainingBullets = magazineSize;
		reloading = false;
		ammoBar.EndReloading();
		ammoBar.SetAmmo(remainingBullets);
	}
	public void Reset()
	{
		remainingBullets = magazineSize;
		ammoBar.SetAmmo(remainingBullets);
	}
}
