using Sandbox;

public sealed class GibManager : Component
{
	[Property] public bool IsGibbing { get; set; } = false;
	[Property] Prop prop { get; set; }
	public SkinnedModelRenderer modelRenderer;
	protected override void OnStart()
	{
		prop = GameObject.Components.Get<Prop>();
		prop.Kill();
	}
	protected override void OnUpdate()
	{

	}
}
