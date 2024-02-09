using System.Linq;
using System.Numerics;
using Sandbox;

public sealed class ViewModelManager : Component
{
	[Property] SkinnedModelRenderer weaponRender {get; set;}
	[Property] public SkinnedModelRenderer armRender { get; set; }
	[Property] Model mp5Model { get; set; }
	[Property] Model shotgunModel { get; set; }
	[Property] Model fistsModel { get; set; }
	private Weapon weapon;
	private Vector3 cameraStartPos;
	[Property] public AnimationGraph punchGraph { get; set; }
	[Property] public AnimationGraph normalGraph { get; set; }
	private CameraComponent viewModelCamera;
	public Model blankModel;
	protected override void OnAwake()
	{
		viewModelCamera = Components.Get<CameraComponent>();
		cameraStartPos = viewModelCamera.GameObject.Transform.LocalPosition;
	}
	protected override void OnStart()
	{
		weapon = GameManager.ActiveScene.GetAllComponents<Weapon>().FirstOrDefault(x => !x.IsProxy);
	}
	protected override void OnUpdate()
	{
		


		
	}
	void ShowSmg()
	{

			weaponRender.Enabled = true;
			armRender.BoneMergeTarget = weaponRender;
			weaponRender.Model = mp5Model;
			armRender.Enabled = true;
			viewModelCamera.Transform.LocalPosition = cameraStartPos;

	}

	void ShowFists()
	{
		armRender.SceneModel.AnimationGraph = punchGraph;
			armRender.BoneMergeTarget = null;
			weaponRender.Enabled = false;
			viewModelCamera.GameObject.Transform.LocalPosition = new Vector3(15, 0, -5);
	}

	void ShowShotgun()
	{
			weaponRender.Enabled = true;
			armRender.BoneMergeTarget = weaponRender;
			weaponRender.Model = shotgunModel;
			armRender.Enabled = true;
			viewModelCamera.Transform.LocalPosition = cameraStartPos;
	}
}
