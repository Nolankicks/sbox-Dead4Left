using System;
using System.Linq;
using Kicks;
using Sandbox;
using Sandbox.Citizen;

public sealed class Badguy : Component
{
	[Property] public SkinnedModelRenderer body { get; set; }
	[Property] public CitizenAnimationHelper citizenAnimationHelper { get; set; }
	[Property] public PlayerController controller { get; set; }
	public Vector3 target;
	public Vector3 WishVelocity;
	[Property] public float Speed { get; set; }
	[Property] public CharacterController characterController { get; set; }
	TimeSince timeSinceHit = 0;
	[Property] public HealthManager healthManager { get; set; }
	
	protected override void OnStart()
	{
		citizenAnimationHelper.HoldType = CitizenAnimationHelper.HoldTypes.Swing;

	}
	protected override void OnUpdate()
	{
		target = controller.GameObject.Transform.Position;
		BuildWishVelocity();
		UpdateMovement();
		UpdateAnimations();
		Trace();
	}

	void BuildWishVelocity()
	{
		WishVelocity = (target - body.Transform.Position).Normal;

		WishVelocity = WishVelocity.Normal;
		WishVelocity = WishVelocity * Speed;
	}
	void UpdateMovement()
	{
		var gravity = Scene.PhysicsWorld.Gravity;
		characterController.ApplyFriction(GetFriction());

		if (characterController.IsOnGround)
		{
			characterController.Accelerate(WishVelocity);
			characterController.Velocity = characterController.Velocity.WithZ(0);
			body.Transform.Rotation = Rotation.LookAt(characterController.Velocity.WithZ(0), Vector3.Up);
		}
		else
		{
			characterController.Velocity += gravity * Time.Delta * 0.5f;
		}
		characterController.Move();

		if (!characterController.IsOnGround)
		{
			characterController.Velocity += gravity * Time.Delta * 0.5f;
		}
	}
	float GetFriction()
	{
		if ( characterController.IsOnGround ) return 6.0f;

		return 0.2f;
	}
	void UpdateAnimations()
	{
		
		if ( target != Vector3.Zero )
		{
			var targetRot = Rotation.LookAt( target.WithZ( Transform.Position.z ) - Transform.Position, Vector3.Up );
			body.Transform.Rotation = Rotation.Slerp( body.Transform.Rotation, targetRot, Time.Delta * 10f );
		}

		citizenAnimationHelper.WithWishVelocity( WishVelocity );
		citizenAnimationHelper.WithVelocity( characterController.Velocity );
		
	}

	void Trace()
	{
		var lookDir = body.Transform.Rotation;
		var tr = Scene.Trace.Ray(body.Transform.Position, body.Transform.Position + lookDir.Forward * 50).WithoutTags("bad").Run();
		if (tr.Hit && tr.GameObject.Tags.Has("player"))
        {
           
            if (timeSinceHit > 2)
            {
            	healthManager.health -= 10;
               	timeSinceHit = 0;
			   	
			   citizenAnimationHelper.Target.Set("b_attack", true);
            }
            
        }
	}
}

