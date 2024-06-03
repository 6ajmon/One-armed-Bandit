using Godot;
using System;

public partial class AmmoBar : ProgressBar
{
	PlayerGun playerGun = null;
	public override void _Ready()
	{
		playerGun = GetParent<PlayerGun>();
		MaxValue = playerGun.magazineSize;
	}
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void SetAmmo(int ammo)
	{
		Value = ammo;
	}
}
