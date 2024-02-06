using System.Numerics;
using Sandbox;

public sealed class ViewModelManager : Component
{
	[Property] SkinnedModelRenderer weaponRender {get; set;}
	[Property] public SkinnedModelRenderer armRender { get; set; }
	[Property] Model mp5Model { get; set; }
	[Property] Model fistsModel { get; set; }
	[Property] Weapon weapon { get; set; }
	private Vector3 cameraStartPos;
	[Property] public AnimationGraph animationGraph { get; set; }
	private CameraComponent viewModelCamera;
	public Model blankModel;
	protected override void OnAwake()
	{
		viewModelCamera = Components.GetInParent<CameraComponent>();
		cameraStartPos = viewModelCamera.GameObject.Transform.LocalPosition;
	}

	protected override void OnUpdate()
	{
		if (weapon.ActiveSlot == 0)
		{
			weaponRender.Model = mp5Model;
			armRender.Enabled = true;
			viewModelCamera.Transform.LocalPosition = cameraStartPos;
		}
		else
		{
			armRender.SceneModel.AnimationGraph = animationGraph;
			armRender.BoneMergeTarget = null;
			weaponRender.Enabled = false;
			viewModelCamera.GameObject.Transform.LocalPosition = new Vector3(15, 0, -5);
		}

		
	}
}
