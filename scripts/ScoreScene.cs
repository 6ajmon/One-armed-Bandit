using Godot;
using System;

public partial class ScoreScene : Panel
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Score Scene Ready");
		this.Hide();
	}
	public void ShowScore()
	{
		GetTree().Paused = true;
		GD.Print("Showing Score");
		GetNode<Label>("PlayerScore1").Text = GameManager.Players[0].Score.ToString();
		GetNode<Label>("PlayerScore2").Text = GameManager.Players[1].Score.ToString();
		this.Show();
	}

	private void OnNextRoundButtonPressed()
	{
		GD.Print("Next Round Button Down");
		var stage = GetParent().GetNode<Stage>("Stage");
		stage.Rpc(nameof(stage.ResetRound));
		Rpc(nameof(HideAndUnpause));
	}
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void HideAndUnpause()
	{
		this.Hide();
		GetTree().Paused = false;
	}
}
