using Sandbox;

public sealed class ViewModelManager : Component
{
	[Property] SkinnedModelRenderer weaponRender {get; set;}
	[Property] public SkinnedModelRenderer armRender { get; set; }
	[Property] Model mp5Model { get; set; }
	[Property] Model fistsModel { get; set; }
	[Property] Weapon weapon { get; set; }



	protected override void OnUpdate()
	{
		if (weapon.ActiveSlot == 0)
		{
			weaponRender.Model = mp5Model;
		}
		else
		{
			weaponRender.Model = fistsModel;
			
		}
		
	}
}
