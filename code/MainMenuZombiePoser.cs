using System;
using Sandbox;
using Sandbox.Citizen;

public sealed class MainMenuZombiePoser : Component
{
	[Property] public CitizenAnimationHelper AnimationHelper { get; set; }
	float GetRandom() => Random.Shared.Float(0, 100);
	protected override void OnStart()
	{
		AnimationHelper.HoldType = CitizenAnimationHelper.HoldTypes.Punch;
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
		
	}
	TimeUntil nextSecond;
	protected override void OnFixedUpdate()
	{
		if (nextSecond)
		{
			var random = GetRandom();

			GetRandom();

			if (random > 70)
			{
				AnimationHelper.Target.Set("b_attack", true);
			}

			nextSecond = 1;
		}
	}
}
