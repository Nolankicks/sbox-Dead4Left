using Sandbox;

public sealed class NetworkedViewmodel : Component
{
	[Property] public GameObject eye { get; set; }
	private Vector3 eyePosition;
	private Rotation eyeRotation;

	protected override void OnUpdate()
	{
		eyeRotation = eye.Transform.Rotation;
		eyePosition = eye.Transform.Position;
		GameObject.Transform.Rotation = eyeRotation;
		GameObject.Transform.Position = eyePosition;
	}
}
