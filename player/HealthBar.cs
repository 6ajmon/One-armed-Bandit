using Godot;
using System;

public partial class HealthBar : ProgressBar
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	
	public void SetHealth(float health)
	{
		Value = health;
	}
}
