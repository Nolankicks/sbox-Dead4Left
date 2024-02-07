using Sandbox;

public sealed class Viewmodel : Component
{
	[Property] public GameObject eye { get; set; }
	[Property] public GameObject arms { get; set; }
	public Rotation rotation;
	public Vector3 vector3;
	protected override void OnAwake()
	{
		rotation = arms.Transform.LocalRotation;
		vector3 = arms.Transform.LocalPosition;
	}
	protected override void OnUpdate()
	{
			if (Input.Pressed("Duck"))
			{
				arms.Transform.Position -= Vector3.Down * -32;
			}
			if (Input.Released("Duck"))
			{
				arms.Transform.LocalPosition = vector3;
				arms.Transform.LocalRotation = rotation;
			}
	}
}
