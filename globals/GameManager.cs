using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node
{
	public static List<PlayerInfo> Players = new();
	public static void AddScore(int id)
	{
		foreach(PlayerInfo player in Players)
		{
			if(!(player.Id == id))
			{
				player.Score++;
			}
		}
	}
}
