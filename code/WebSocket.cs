using Sandbox;
using System;
using System.Threading.Tasks;
public sealed class SocketShitter : Component
{
	[Property] public string ConnectionURL { get; set; }
	public WebSocket Socket {get; set;}

	protected override void OnStart()
	{
		Socket = new WebSocket();
		Socket.OnMessageReceived += HandleMessageReceived;
		_ = Connect();
		
	}
	private async Task Connect()
	{
		await Socket.Connect(ConnectionURL);
		await Send("Hello World");
	}

	private async Task Send(string message)
	{
		await Socket.Send(message);
	}

	private void HandleMessageReceived(string message)
	{
		Log.Info(message);
	}
}
