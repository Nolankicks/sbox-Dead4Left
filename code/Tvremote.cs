using System.Linq;
using Sandbox;
using Sandbox.UI;

public sealed class Tvremote : Component
{
	public WorldInput worldInput = new WorldInput();
	[Property] public bool UseScreen { get; set; } = false;
	protected override void OnUpdate()
	{
		if (IsProxy) return;
		var screen = Game.ActiveScene.GetAllComponents<VideoPlayer>().FirstOrDefault();
		if (UseScreen)
		{
			screen.Url = "https://www.youtube.com/watch?v=6n3pFFPSlW4";
		}
	}
}
