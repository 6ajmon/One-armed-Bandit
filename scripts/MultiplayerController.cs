using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MultiplayerController : Control
{
	[Export] private int port = 1234;
	private string ip = "127.0.0.1";
	private ENetMultiplayerPeer peer;
	// Called when the node enters the scene tree for the first time.
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
		RpcId(1, "sendPlayerInformation", GetNode<LineEdit>("LineEdit").Text, Multiplayer.GetUniqueId());
        GD.Print("Connected to Server");
    }


    private void PeerDisconnected(long id)
    {
        GD.Print("Peer Disconnected" + id.ToString());
		GameManager.Players.Remove(GameManager.Players.Where(x => x.Id == id).First<PlayerInfo>());
		var players = GetTree().GetNodesInGroup("Player");
		foreach(Player player in players)
		{
			if(player.Name == id.ToString())
			{
				player.QueueFree();
			}
		}
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
	public void PeerConnected(long id)
	{
		GD.Print("Peer Connected" + id.ToString());
	}

	public void _on_host_button_down()
	{
		peer = new ENetMultiplayerPeer();
		var error = peer.CreateServer(port, 2);
		if (error != Error.Ok)
		{
			GD.Print("Error creating server" + error.ToString());
			return;
		}
		peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);

		Multiplayer.MultiplayerPeer = peer;
		sendPlayerInformation(GetNode<LineEdit>("LineEdit").Text, 1);
		GD.Print("Server Created waiting for players to join");
	}
	public void _on_join_button_down()
	{
		peer = new();
		peer.CreateClient(ip, port);

		peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder);
		Multiplayer.MultiplayerPeer = peer;
		GD.Print("Joining Server");	
	}
	public void _on_start_game_button_down()
	{
		Rpc("startGame");
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void startGame()
	{
		foreach(var player in GameManager.Players)
		{
			GD.Print(player.Name + " is playing");
		}
		var scene = ResourceLoader.Load<PackedScene>("res://scenes/stage.tscn").Instantiate() as Node2D;
		GetTree().Root.AddChild(scene);
		this.Hide();
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
				Rpc("sendPlayerInformation", player.Name, player.Id);
			}
		}
	}
}
