using System;
using System.Collections.Generic;
using System.IO.Pipes;
using Sandbox;
using Sandbox.UI;

public sealed class Weapon : Component
{
	[Property] public Image WeaponImage {get; set;}

	
	[Property] public List<Texture> InventoryImages {get; set;} = new List<Texture>()
	{

	};
	[Property] public List<string> Weapons {get; set;} = new List<string>()
	{
		"weapon_smg"
	};
	
	public int ActiveSlot = 0;
	public int Slots => 9;
	public int Img => 9;
	public string[] Inventory = new string[9];
	[Property] public bool HasShotgun {get; set;} = false;
	protected override void OnAwake()
	{
		for (int i = 0; i < Inventory.Length; i++)
		{
			Inventory[i] = "weapon_fists";
		}
		Inventory[0] = "weapon_smg";
		Inventory[1] = "";
		Inventory[2] = "";
		Inventory[3] = "";
		Inventory[4] = "";
		Inventory[5] = "";
		Inventory[6] = "";
		Inventory[7] = "";
		Inventory[8] = "";
	}
	protected override void OnUpdate()
	{
		if (Input.MouseWheel.y != 0)
		{
			ActiveSlot = (ActiveSlot + Math.Sign(Input.MouseWheel.y)) % Inventory.Length;
		}
		if (ActiveSlot < 0)
		{
			ActiveSlot = Slots - 1;
		}
		

		if (Inventory[ActiveSlot] == "weapon_smg")
		{
			Log.Info("SMG");
		}
		if (HasShotgun)
		{
			Inventory[1] = "weapon_shotgun";
		}
	}
}

