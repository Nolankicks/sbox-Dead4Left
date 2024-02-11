using Sandbox;

public sealed class ActiveWeapon : Component
{
	[Property] public Item Item { get; set; }
	[Property] public SkinnedModelRenderer arms { get; set; }
	protected override void OnStart()
	{
		if (Item is null) return;
		var weapon = SceneUtility.GetPrefabScene(Item.weaponPrefab);
		var weaponClone = weapon.Clone(GameObject.Transform.LocalPosition);
		weaponClone.SetParent(arms.GameObject);
		//var modelRenderer = weaponClone.Components.Get<SkinnedModelRenderer>();
		//arms.BoneMergeTarget = modelRenderer;

	}
}
