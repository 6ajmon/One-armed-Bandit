using Godot;
using System;

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
        GD.Print("Connected to Server");
    }


    private void PeerDisconnected(long id)
    {
        GD.Print("Peer Disconnected" + id.ToString());
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
		var scene = ResourceLoader.Load<PackedScene>("res://scenes/stage.tscn").Instantiate() as Node2D;
		GetTree().Root.AddChild(scene);
		this.Hide();
	}
}
