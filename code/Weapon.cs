using System;
using System.Collections.Generic;
using Sandbox;

public sealed class Weapon : Component
{
	[Property] public List<string> Inventory {get; set;} = new List<string>()
	{
		"weapon_SMG"
	};
	public int ActiveSlot = 0;
	public int Slots => 9;
	protected override void OnUpdate()
	{
		if (Input.MouseWheel.y != 0)
		{
			ActiveSlot = (ActiveSlot + Math.Sign(Input.MouseWheel.y)) % Slots;
		}
		if (ActiveSlot < 0)
		{
			ActiveSlot = Slots - 1;
		}
		
		Log.Info(ActiveSlot);
	}
}
