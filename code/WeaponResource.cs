using Microsoft.VisualBasic;
using Sandbox;

[GameResource( "Item", "item", "A item game resource", Icon = "track_changes" ) ]
public class Item : GameResource
{
	public PrefabFile weaponPrefab { get; set; }
	public string weaponName { get; set; }
	[Property, Range(0, 500)] public float weaponDamage { get; set; } = 50;
	public Model weaponModel { get; set; }
	[Property, Range(0, 120)] public float ammo { get; set; } = 30;
	[Property, Range(0, 120)] public float fullAmmo { get; set; } = 60;
	[Property, Range(0.1f, 1)] public float fireRate { get; set; } = 0.1f;
}


