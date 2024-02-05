using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;

public sealed class Weapon : Component
{
	[Property] public Image WeaponImage {get; set;}
	[Property] public List<string> Inventory {get; set;} = new List<string>()
	{
		"weapon_SMG"
	};
	
	[Property] public List<string> InventoryImages {get; set;} = new List<string>()
	{

	};
	
	public int ActiveSlot = 0;
	public int Slots => 9;
	public int Img => 9;
	
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

