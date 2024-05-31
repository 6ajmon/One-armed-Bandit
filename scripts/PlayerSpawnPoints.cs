using Godot;
using System;

public partial class PlayerSpawnPoints : Node2D
{
	[Export] public PackedScene PlayerScene;
	public override void _Ready()
	{
		SpawnPlayers();
	}
	private void SpawnPlayers()
	{
		int index = 0;
		foreach (var player in GameManager.Players){

			Player currentPlayer = PlayerScene.Instantiate() as Player;
			currentPlayer.Name = player.Id.ToString();
			GetTree().Root.AddChild(currentPlayer, true);
			foreach(Node2D point in GetChildren())
			{
				if(point is Marker2D && point.Name == "PlayerSpawnPoint" + index.ToString())
				{
					currentPlayer.GlobalPosition = point.GlobalPosition;
					if (index == 1){
						currentPlayer.SetUpSecondPlayer();
					}
				}
			}
			index++;
		}
	}
}