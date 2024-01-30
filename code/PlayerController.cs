using System;
using System.Linq;
using System.Net.Http;
using Sandbox;
using Sandbox.Citizen;
namespace Kicks;


public sealed class PlayerController : Component, IHealthComponent
{

	[Property] public GameObject body { get; set; }
	public Vector3 WishVelocity = Vector3.Zero;
	[Sync, Property] public float MaxHealth { get; private set; } = 100f;
	[Sync] public float Health { get; private set; } = 100f;
	[Property] public CitizenAnimationHelper animationHelper { get; set; }
	[Property] public float distance { get; set; } = 100;
	[Property] public float GroundControl { get; set; } = 4;
	[Property] public float AirControl { get; set; } = 0.1f;
	[Property] public float Speed { get; set; } = 160;
	[Property] public float RunSpeed { get; set; } = 320;
	[Property] public float CrouchSpeed { get; set; } = 90;
	[Property] public float MaxForce { get; set; } = 50;
	[Property] public float JumpForce { get; set; } = 400;
	[Property] public GameObject eye { get; set; }
	[Property] public SceneFile respawnScene { get; set; }
	
	[Property] public bool AbleToCrouch;
	public bool IsCrouching = false;
	public bool IsSprinting = false;
	public bool IsFirstPerson => distance == 0f;
	private Vector3 CurrentOffset = Vector3.Zero;
	private CharacterController controller;

	[Property] public CameraComponent camera { get; set; }
	private ModelRenderer bodyRenderer;
	protected override void OnAwake()
	{
		controller = Components.Get<CharacterController>();
		
		bodyRenderer = body.Components.Get<ModelRenderer>();
	}
	protected override void OnUpdate()
	{
		//rotate body
		var eyeAngles = eye.Transform.Rotation.Angles();
		eyeAngles.pitch += Input.MouseDelta.y * 0.1f;
		eyeAngles.yaw -= Input.MouseDelta.x * 0.1f;
		eyeAngles.roll = 0;
		eyeAngles.pitch = eyeAngles.pitch.Clamp(-89.9f, 89.9f);
		eye.Transform.Rotation = eyeAngles.ToRotation();
		Rotatebody();
		UpdateAnimations();
		if (Input.Pressed("jump")) Jump();
		CrouchUpdate();
		IsSprinting = Input.Down("Run");

		var tartetOffeset = Vector3.Zero;
		if (IsCrouching) tartetOffeset += Vector3.Down * 32f;
		CurrentOffset = Vector3.Lerp(CurrentOffset, tartetOffeset, Time.Delta * 10f);
		


		if (camera is not null)
		{
			var camPos = eye.Transform.Position + CurrentOffset;
			if (!IsFirstPerson)
			{
			
			var camFoward = eyeAngles.ToRotation().Forward;
			var camTrace = Scene.Trace.Ray(camPos, camPos - (camFoward * distance))
			.WithoutTags("player", "trigger")
			.Run();

			if (camTrace.Hit)
			{
				camPos = camTrace.HitPosition + camTrace.Normal;
			}
			else
			{
				camPos = camTrace.EndPosition;
			}
			bodyRenderer.RenderType = ModelRenderer.ShadowRenderType.On;

		}
			if (IsFirstPerson)
			{
				var renderMode = ModelRenderer.ShadowRenderType.On;
				if (!IsProxy) renderMode = ModelRenderer.ShadowRenderType.ShadowsOnly;
				camera.FieldOfView = 90;
			}
			camera.Transform.Position = camPos;
			camera.Transform.Rotation = eyeAngles.ToRotation();

		}
		
	}

		
	
	protected override void OnFixedUpdate()
	{
		BuildWishVelocity();
		
		Move();
	}

	void BuildWishVelocity()
	{
		WishVelocity = 0;
		var rot = eye.Transform.Rotation;
		if (Input.Down("Forward")) WishVelocity += rot.Forward;
		if (Input.Down("Backward")) WishVelocity += rot.Backward;
		if (Input.Down("Left")) WishVelocity += rot.Left;
		if (Input.Down("Right")) WishVelocity += rot.Right;

		WishVelocity = WishVelocity.WithZ(0);
		if (!WishVelocity.IsNearZeroLength) WishVelocity = WishVelocity.Normal;
		if (IsCrouching) WishVelocity *= CrouchSpeed;
		else if (IsSprinting) WishVelocity *= RunSpeed;
		else WishVelocity *= Speed;
	}
	
	void Move()
	{
		// Get gravity
		var gravity = Scene.PhysicsWorld.Gravity;
		if (controller.IsOnGround)
		{
			controller.Velocity = controller.Velocity.WithZ(0);
			controller.Accelerate(WishVelocity);
			controller.ApplyFriction(GroundControl);
		}
		else
		{
			controller.Velocity += gravity * Time.Delta * 0.5f;
			controller.Accelerate(WishVelocity.ClampLength(MaxForce));
			controller.ApplyFriction(AirControl);

		}
		controller.Move();

		if(!controller.IsOnGround)
		{
			controller.Velocity += gravity * Time.Delta * 0.5f;
		}
		else
		{
			controller.Velocity = controller.Velocity.WithZ(0);
		}
	}

	void Rotatebody()
	{
		if (body is null) return;

		var targetAngle = new Angles(0, eye.Transform.Rotation.Yaw(), 0).ToRotation();
		float rotateDiff = body.Transform.Rotation.Distance(targetAngle);

		if (rotateDiff > 50f  || controller.Velocity.Length > 10f)
		{
			body.Transform.Rotation = Rotation.Lerp(body.Transform.Rotation, targetAngle, Time.Delta * 2f);
		}
	}

	void UpdateAnimations()
	{
		if(animationHelper is null) return;

		animationHelper.WithWishVelocity(WishVelocity);
		animationHelper.WithVelocity(controller.Velocity);
		animationHelper.AimAngle = eye.Transform.Rotation;
		animationHelper.IsGrounded = controller.IsOnGround;
		animationHelper.WithLook(eye.Transform.Rotation.Forward, 1f, 0.75f, 0.5f);
		animationHelper.MoveStyle = CitizenAnimationHelper.MoveStyles.Auto;
		animationHelper.DuckLevel = IsCrouching ? 1f : 0f;
	}
	void Jump()
	{
		if (!controller.IsOnGround) return;

		controller.Punch(Vector3.Up * JumpForce);
		animationHelper?.TriggerJump();
	}

	void CrouchUpdate()
	{
		var crouchTr = Scene.Trace.Ray(body.Transform.Position, body.Transform.Position + body.Transform.Rotation.Up * 50f)
			.WithoutTags("player", "trigger")
			.Run();
		if (controller is null) return;

			if(Input.Pressed("Duck") && !IsCrouching)
			{
				IsCrouching = true;
				controller.Height /= 2;
			}
			if (Input.Released("Duck") && IsCrouching && !crouchTr.Hit)
			{
				IsCrouching = false;
				controller.Height *= 2;
			}




	}
	public void TakeDamage(float damage)
	{
		
		Health -= damage;
		
		Log.Info(Health);

		if (Health < 0)
		{
			Health = 0;
		}
		if (Health == 0)
		{
			GameManager.ActiveScene.Load(respawnScene);
		}
}
}


