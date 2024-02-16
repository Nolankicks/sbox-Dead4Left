using System;
using System.Net.Quic;
using System.Threading.Tasks;
using Sandbox;

public sealed class Async : Component
{
	protected override void OnEnabled()
	{
		_ = Print( 2f);
	}

	async Task<string> GetMessage()
	{
		string quote = await Http.RequestStringAsync( "https://api.kanye.rest/" );

		return quote;
	}
	async Task Print(float waitSeconds)
	{
		while (true)
		{
		await Task.DelaySeconds( waitSeconds );
		string quote = await GetMessage();
		Log.Info( quote );
		}
	}
}
