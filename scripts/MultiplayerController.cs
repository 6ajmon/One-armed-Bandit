using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MultiplayerController : Panel
{
	[Export] private int port = 1234;
	private string ip = "127.0.0.1";
	private ENetMultiplayerPeer peer;
	private bool isPlayerJoined = false;
	private DataBaseController dataBaseController = new();
	public override void _Ready()
	{
		Multiplayer.PeerConnected += PeerConnected;
		Multiplayer.PeerDisconnected += PeerDisconnected;
		Multiplayer.ConnectedToServer += ConnectedToServer;
		Multiplayer.ConnectionFailed += ConnectionFailed;
	}

    private void ConnectionFailed()
    {
        GD.Print("Connection Failed");
    }

    private void ConnectedToServer()
    {
		string name;
		if (GetNode<LineEdit>("Name").Text == "")
			name = Multiplayer.GetUniqueId().ToString();
		else
			name = GetNode<LineEdit>("Name").Text;
		RpcId(1, nameof(sendPlayerInformation), name, Multiplayer.GetUniqueId());
        GD.Print("Connected to Server");
    }

    private void PeerDisconnected(long id)
    {
        GD.Print("Peer Disconnected: " + id.ToString());
		GameManager.Players.Remove(GameManager.Players.Where(x => x.Id == id).First<PlayerInfo>());
		var players = GetTree().GetNodesInGroup("Player");
		foreach(var player in players)
		{
			if(player.Name == id.ToString())
			{
				player.QueueFree();
				GD.Print("Player Removed " + id.ToString());
				break;
			}
		}
    }

	public void PeerConnected(long id)
	{
		isPlayerJoined = true;
		GD.Print("Peer Connected: " + id.ToString());
	}

	public void _on_host_button_down()
	{
		port = int.Parse(GetNode<LineEdit>("Port").Text);
		ip = GetNode<LineEdit>("Ip").Text;
		peer = new ENetMultiplayerPeer();
		var error = peer.CreateServer(port, 2);
		if (error != Error.Ok)
		{
			GD.Print("Error creating server: " + error.ToString());
			return;
		}
		peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);

		Multiplayer.MultiplayerPeer = peer;
		string name;
		if (GetNode<LineEdit>("Name").Text == "")
			name = "1";
		else
			name = GetNode<LineEdit>("Name").Text;
		sendPlayerInformation(name, 1);
		GD.Print("Server Created waiting for players to join");
	}

	public void _on_join_button_down()
	{
		port = int.Parse(GetNode<LineEdit>("Port").Text);
		ip = GetNode<LineEdit>("Ip").Text;
		peer = new();
		peer.CreateClient(ip, port);

		peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		Multiplayer.MultiplayerPeer = peer;
		GD.Print("Joining Server");	
	}

	public void _on_start_game_button_down()
	{
		if(!isPlayerJoined)
		{
			GD.Print("Wait for other player to join");
			return;
		}
		Rpc(nameof(startGame));
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void startGame()
	{
		if (Multiplayer.IsServer())
		{
			foreach(var player in GameManager.Players)
			{
				dataBaseController.InsertPlayer(player);
			}
		}
		
		var scene = ResourceLoader.Load<PackedScene>("res://scenes/Stage1.tscn").Instantiate() as Node2D;
		var scoreScene = ResourceLoader.Load<PackedScene>("res://scenes/ScoreScene.tscn").Instantiate() as Control;
		GetTree().Root.AddChild(scene);
		GetTree().Root.AddChild(scoreScene);
		QueueFree();
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void sendPlayerInformation(string name, int id){
		PlayerInfo playerInfo = new(){
			Name = name,
			Id = id
		};
		if(!GameManager.Players.Contains(playerInfo))
		{
			GameManager.Players.Add(playerInfo);
		}

		if(Multiplayer.IsServer())
		{
			foreach(var player in GameManager.Players)
			{
				Rpc(nameof(sendPlayerInformation), player.Name, player.Id);
			}
		}
	}
	
}
