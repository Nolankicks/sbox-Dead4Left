using Sandbox;

[GameResource( "Item", "item", "Describes an item definition", Icon = "track_changes" ) ]
public class Item : GameResource
{
	public PrefabFile weaponPrefab { get; set; }
	public string weaponName { get; set; }
	

}
