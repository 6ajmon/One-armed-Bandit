using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameManager : Node
{
	public static DataBaseController dataBaseController = new();
	public static List<PlayerInfo> Players = new();

	
	public static void AddScore(int id)
	{
		foreach(PlayerInfo player in Players)
		{
			if(!(player.Id == id))
			{
				player.Score++;
				dataBaseController.UpdateHighScore(player);
			}
		}
	}

}
