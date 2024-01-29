using Sandbox;

public sealed class AmmoPickup : Component, Component.ITriggerListener
{
	protected override void OnUpdate()
	{

	}

	void ITriggerListener.OnTriggerEnter(Sandbox.Collider other)
	{
		if (!other.GameObject.IsValid) return;

		var ammo = other.GameObject.Components.GetInParentOrSelf<MP5>();
		Log.Info(ammo);
		if (ammo.IsValid)
		{
			ammo.fullAmmo += 30;
		}
	}

	void ITriggerListener.OnTriggerExit(Sandbox.Collider other)
	{

	}
}
