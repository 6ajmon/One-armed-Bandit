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
					var playerNode = point.GetChild<Player>(0);
					playerNode.GlobalPosition = point.GlobalPosition;
					playerNode.Reset();
					if (index != 0)
						playerNode.GetNode<Sprite2D>("PlayerSprite").FlipH = true;
				}
			}
			index++;
		}
	}
}