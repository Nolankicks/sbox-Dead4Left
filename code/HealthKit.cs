using System.Linq;
using System.Threading.Tasks;
using Kicks;
using Sandbox;

public sealed class HealthKit : Component
{
	public PlayerController player;
	public int PatchUptime = 5;
	public bool Healing = false;
	public bool CanHeal = true;
	protected override void OnStart()
	{
		player = GameManager.ActiveScene.GetAllComponents<PlayerController>().FirstOrDefault(x => !x.IsProxy);
	}
	protected override void OnUpdate()
	{
		if (Input.Pressed("attack1") && !IsProxy && CanHeal)
		{
			_ = Heal();
			Healing = true;
			CanHeal = false;
		}
	}

	async Task Heal()
	{
		await Task.DelayRealtimeSeconds(PatchUptime);
		player.Health += 25;
		GameObject.Destroy();
	}
}
