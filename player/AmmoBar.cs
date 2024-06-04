using Godot;
using System;

public partial class AmmoBar : ProgressBar
{
	private PlayerGun playerGun = null;
	private Timer reloadTimer = null;
	private bool isStarted = false;
	public override void _Ready()
	{
		playerGun = GetParent<PlayerGun>();
		MaxValue = playerGun.magazineSize;
		reloadTimer = playerGun.GetNode<Timer>("Reload");
	}
	public override void _Process(double delta)
	{
		if (isStarted)
		{
			Value = reloadTimer.WaitTime - reloadTimer.TimeLeft;
		}
	}
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void SetAmmo(int ammo)
	{
		Value = ammo;
	}
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void StartReloading()
	{
		isStarted = true;
		MaxValue = reloadTimer.WaitTime;
	}
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void EndReloading()
	{
		isStarted = false;
		MaxValue = playerGun.magazineSize;
	}
}
