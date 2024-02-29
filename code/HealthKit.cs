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
using static Weapon;
public sealed class HealthKit : Component
{
	public PlayerController player;
	public int PatchUptime = 5;
	public bool Healing = false;
	public bool CanHeal = true;
	public TimeUntil HealCooldown;
	public Weapon weapon;
	public int Amount = 1;
	public WeaponData weaponData;
	protected override void OnStart()
	{
		player = GameManager.ActiveScene.GetAllComponents<PlayerController>().FirstOrDefault(x => !x.IsProxy);
		weapon = GameManager.ActiveScene.GetAllComponents<Weapon>().FirstOrDefault(x => !x.IsProxy);
		Amount = 1;
	}
	protected override void OnUpdate()
	{
		if (Input.Pressed("attack1") && !IsProxy && CanHeal && Amount > 0 && GameObject.Enabled == true)
		{
			_ = Heal();
			Healing = true;
			CanHeal = false;
			HealCooldown = PatchUptime;
			Amount -= 0;
			weapon.AbleToSwitch = false;
		}
		

	}

	async Task Heal()
	{
		await Task.DelayRealtimeSeconds(PatchUptime);
		
		player.Health += 25;
		Healing = false;
		weapon.AbleToSwitch = true;
	}
			/*protected override void OnDestroy()
			{
			 weapon.WeaponList[Array.IndexOf(weapon.WeaponList, GameObject)] = null;
			}*/
}
