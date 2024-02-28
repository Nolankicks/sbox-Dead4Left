using System.Linq;
using System.Threading.Tasks;
using Kicks;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using Kicks;
using Sandbox;
using Sandbox.UI;
using Sandbox.Citizen;
public sealed class HealthKit : Component
{
	public PlayerController player;
	public int PatchUptime = 5;
	public bool Healing = false;
	public bool CanHeal = true;
	public TimeUntil HealCooldown;
	public Weapon weapon;
	protected override void OnStart()
	{
		player = GameManager.ActiveScene.GetAllComponents<PlayerController>().FirstOrDefault(x => !x.IsProxy);
		weapon = GameManager.ActiveScene.GetAllComponents<Weapon>().FirstOrDefault(x => !x.IsProxy);
	}
	protected override void OnUpdate()
	{
		if (Input.Pressed("attack1") && !IsProxy && CanHeal)
		{
			_ = Heal();
			Healing = true;
			CanHeal = false;
			HealCooldown = PatchUptime;
		}
		

	}

	async Task Heal()
	{
		await Task.DelayRealtimeSeconds(PatchUptime);
		
		player.Health += 25;
		GameObject.Destroy();
	}
protected override void OnDestroy()
	{
			 weapon.WeaponList[Array.IndexOf(weapon.WeaponList, GameObject)] = null;
	}
}
