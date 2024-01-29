using Sandbox;

public sealed class HealthManager : Component
{
	public float health { get; set; } = 100;
	[Property] public Manager manager { get; set; }
	protected override void OnUpdate()
	{
		if (health <= 0)
		{
			manager.GameEnd();
		}
	}
}
