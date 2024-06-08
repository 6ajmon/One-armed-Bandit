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
	private int countdown = 4;
	public override void _Ready()
	{
		Multiplayer.PeerConnected += PeerConnected;
		Multiplayer.PeerDisconnected += PeerDisconnected;
		Multiplayer.ConnectedToServer += ConnectedToServer;
		Multiplayer.ConnectionFailed += ConnectionFailed;
	}

	private void ConnectionFailed()
	{
		PrintLog("Connection Failed");
	}

	private void ConnectedToServer()
	{
		string name;
		if (GetNode<LineEdit>("Name").Text == "")
			name = Multiplayer.GetUniqueId().ToString();
		else
			name = GetNode<LineEdit>("Name").Text;
		RpcId(1, nameof(sendPlayerInformation), name, Multiplayer.GetUniqueId());
		PrintLog("Connected to Server");
	}

	private void PeerDisconnected(long id)
	{
		PrintLog("Peer Disconnected: " + id.ToString());
		GameManager.Players.Remove(GameManager.Players.Where(x => x.Id == id).First<PlayerInfo>());
		var players = GetTree().GetNodesInGroup("Player");
		foreach(var player in players)
		{
			if(player.Name == id.ToString())
			{
				player.QueueFree();
				break;
			}
		}
	}

	public void PeerConnected(long id)
	{
		isPlayerJoined = true;
		if (id == 1)
			PrintLog("Connected to player. (UUID: " + id.ToString() + ")");
		else
			PrintLog("Player connected. (UUID: " + id.ToString() + ")");
	}

	public void _on_host_button_down()
	{
		port = int.Parse(GetNode<LineEdit>("Port").Text);
		ip = GetNode<LineEdit>("Ip").Text;
		peer = new();
		var error = peer.CreateServer(port, 2);
		if (error != Error.Ok)
		{
			PrintLog("Error creating server: " + error.ToString());
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
		PrintLog("Hosting Server");
		PrintLog("Port: " + port.ToString() + " Ip: " + ip);
	}

	public void _on_join_button_down()
	{
		port = int.Parse(GetNode<LineEdit>("Port").Text);
		ip = GetNode<LineEdit>("Ip").Text;
		peer = new();
		var error = peer.CreateClient(ip, port);
		if (error != Error.Ok)
		{
			PrintLog("Error joining server: " + error.ToString());
			return;
		}
		peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		Multiplayer.MultiplayerPeer = peer;
	}

	public void _on_start_game_button_down()
	{
		if(!isPlayerJoined)
		{
			PrintLog("Wait for other player to join");
			return;
		}
		Rpc(nameof(startGameCountdown));
	}
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	private void startGameCountdown()
	{
		PrintLog("Game starts in " + (countdown - 1).ToString() + " seconds");
		countdown--;
		if(countdown == 0)
		{
			Rpc(nameof(startGame));
		}
		else
		{
			Timer timer = new()
			{
				WaitTime = 1.0f,
				OneShot = true
			};
			timer.Timeout += startGameCountdown;
			AddChild(timer);
			timer.Start();
		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void startGame()
	{
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
	private void PrintLog(string message)
	{
		var logs = GetNode<TextEdit>("Logs");
		logs.Text += message + "\n";
		logs.ScrollVertical = double.MaxValue;
	}
}
