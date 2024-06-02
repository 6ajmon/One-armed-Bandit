using Godot;
using System;

public partial class PlayerSpawnPoints : Node2D
{
	[Export] public PackedScene PlayerScene;
	public override void _Ready()
	{
		SpawnPlayers();
	}
	public void SpawnPlayers()
	{
		GD.Print("Spawning Players");
		int index = 0;
		foreach (var player in GameManager.Players){

			Player currentPlayer = PlayerScene.Instantiate() as Player;
			currentPlayer.Name = player.Id.ToString();
			foreach(Node2D point in GetChildren())
			{
				if(point is Marker2D && point.Name == "PlayerSpawnPoint" + index.ToString())
				{
					if (point.GetChildren().Count == 0)
						point.AddChild(currentPlayer, true);
					point.GetChild<Player>(0).GlobalPosition = point.GlobalPosition;
					if (index != 0)
						point.GetChild<Player>(0).GetNode<Sprite2D>("PlayerSprite").FlipH = true;
				}
			}
			index++;
		}
	}
}