using Godot;
using System;

public partial class ScoreScene : Panel
{
	public static DataBaseController dataBaseController = new();
	public override void _Ready()
	{
		GD.Print("Score Scene Ready");
		this.Hide();
	}
	public void ShowScore()
	{
		GetTree().Paused = true;
		foreach (var player in GameManager.Players)
			if (Multiplayer.IsServer())
				dataBaseController.UpdateHighScore(player);
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
	private void OnEndGameButtonPressed()
	{
		Rpc(nameof(HideAndUnpause));
		Rpc(nameof(SendQuitInformation));
		if (Multiplayer.IsServer())
			dataBaseController.UpdateTotalScores(GameManager.Players[0], GameManager.Players[1]);
		GetTree().Quit();
	}
	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void SendQuitInformation()
	{
		if (Multiplayer.IsServer())
			dataBaseController.UpdateTotalScores(GameManager.Players[0], GameManager.Players[1]);
		foreach (Node child in GetTree().Root.GetChildren())
		{
			if (child is not GameManager)
				child.QueueFree();
		}
		var mainMenuScene = ResourceLoader.Load<PackedScene>("res://scenes/MultiplayerController.tscn").Instantiate() as Panel;
		GetTree().Root.AddChild(mainMenuScene);
	}
}
