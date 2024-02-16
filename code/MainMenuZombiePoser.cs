using Sandbox;
using Sandbox.Citizen;

public sealed class MainMenuZombiePoser : Component
{
	[Property] public CitizenAnimationHelper AnimationHelper { get; set; }
	protected override void OnStart()
	{
		AnimationHelper.HoldType = CitizenAnimationHelper.HoldTypes.Punch;
	}
}
