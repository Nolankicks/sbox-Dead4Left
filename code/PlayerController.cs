using System;
using System.Linq;
using System.Net.Http;
using Sandbox;
using Sandbox.Citizen;
namespace Kicks;


public sealed class PlayerController : Component, IHealthComponent, IScoreComponent
{
	[Sync] public float Health { get; set; } = 100;
	[Sync] public float MaxHealth { get; set; } = 100;
	[Sync] public long Score { get; set; } = 0;
	[Sync] public long HighScore { get; set; } = 0;
	[Property] public float CrouchSpeed { get; set; } = 64.0f;
	[Property] public float WalkSpeed { get; set; } = 190f;
	[Property] public float RunSpeed { get; set; } = 190f;
	[Property] public float SprintSpeed { get; set; } = 320f;
	[Property] public GameObject eye { get; set; }
	[Property] public SkinnedModelRenderer body { get; set; }
	[Property] public CharacterController controller { get; set; }
	[Property] public CitizenAnimationHelper animationHelper { get; set; }
	[Sync] public long SteamId { get; set; }
	[Sync] public bool Crouching { get; set; }
	[Sync] public Angles EyeAngles { get; set; }
	[Sync] public Vector3 WishVelocity { get; set; }
	[Property] public Weapon weapon { get; set; }
	public bool WishCrouch;
	public CameraComponent camera;
	[Property] public float EyeHight = 64;

	protected override void OnStart()
	{
		var spawnPoints = Scene.GetAllComponents<SpawnPoint>().ToArray();
		var randomSpawnPoint = Random.Shared.FromArray( spawnPoints );
		GameObject.Transform.Position = randomSpawnPoint.Transform.Position;
	}
	protected override void OnUpdate()
	{
		if (!IsProxy)
		{
			MouseInput();
			Transform.Rotation = new Angles(0 , EyeAngles.yaw, 0);
		}
		UpdateAnimation();
		var eyeRot = eye.Transform.Rotation.Angles();
		eyeRot.pitch = Input.MouseDelta.y;
		eyeRot.yaw = Input.MouseDelta.x;

		
	}

	protected override void OnFixedUpdate()
	{
		if (IsProxy) return;
		Movement();
		Crouch();
	}
	private void MouseInput()
	{
		var e = EyeAngles;
		e += Input.AnalogLook;
		e.pitch = e.pitch.Clamp( -90, 90 );
		e.roll = 0;
		EyeAngles = e;
	}

	float CurrentMoveSpeed
	{
		get
		{
			if ( Crouching ) return CrouchSpeed;
			if ( Input.Down( "run" ) ) return SprintSpeed;
			if ( Input.Down( "walk" ) ) return WalkSpeed;

			return RunSpeed;
		}
	}

	float GetFriction()
	{
		if ( controller.IsOnGround ) return 6.0f;

		// air friction
		return 0.2f;
	}
	bool CanUnCrouch()
	{
		if ( !Crouching ) return true;
		
		var tr = controller.TraceDirection( Vector3.Up * 32 );
		return !tr.Hit;
	}
	void Crouch()
	{
		if (Input.Down("duck") && CanUnCrouch())
		{
		 controller.Height = 36;
		 Crouching = true;
		}
		else
		{
			Crouching = false;
			controller.Height = 64;
		}
	}
	public RealTimeSince jumpTime;
	private void Movement()
	{
		if (controller is null) return;

		var cc = controller;

		Vector3 halfgrav = Scene.PhysicsWorld.Gravity * Time.Delta * 0.5f;

		WishVelocity = Input.AnalogMove;
		if (Input.Pressed("jump") && cc.IsOnGround)
		{
			cc.Punch(Vector3.Up * 300);
			
		}
		if (!WishVelocity.IsNearlyZero())
		{
			WishVelocity = new Angles(0, EyeAngles.yaw, 0).ToRotation() * WishVelocity;
			WishVelocity = WishVelocity.WithZ(0);
			WishVelocity = WishVelocity.ClampLength(1);
			WishVelocity *= CurrentMoveSpeed;

			if (!cc.IsOnGround)
			{
				WishVelocity = WishVelocity.ClampLength(50);
			}
		}

		cc.ApplyFriction(GetFriction());

		if (cc.IsOnGround)
		{
			cc.Accelerate(WishVelocity);
			cc.Velocity = controller.Velocity.WithZ(0);
			jumpTime = 0;
		}
		else
		{
			cc.Velocity += halfgrav;
			cc.Accelerate(WishVelocity);
		}
		cc.Move();
		if ( !cc.IsOnGround )
		{
			cc.Velocity += halfgrav;
		}
		else
		{
			cc.Velocity = cc.Velocity.WithZ( 0 );
		}
	}

	
	

		private void UpdateCamera()
	{
		camera = Scene.GetAllComponents<CameraComponent>().Where( x => x.IsMainCamera ).FirstOrDefault();
		if ( camera is null ) return;
		
		var targetEyeHeight = Crouching ? 32 : 64;
		EyeHight = EyeHight.LerpTo( targetEyeHeight, RealTime.Delta * 10.0f );

		var targetCameraPos = Transform.Position + new Vector3( 0, 0, EyeHight );

		// smooth view z, so when going up and down stairs or ducking, it's smooth af
		
		eye.Transform.Rotation = EyeAngles;
		camera.Transform.Position = targetCameraPos;
		camera.Transform.Rotation = EyeAngles;
	}
	protected override void OnPreRender()
	{
		UpdateBodyVisibility();

		if ( IsProxy )
			return;

		UpdateCamera();
	}
		
	private void UpdateBodyVisibility()
	{
		if ( animationHelper is null )
			return;
		var renderMode = ModelRenderer.ShadowRenderType.On;
		if ( !IsProxy ) renderMode = ModelRenderer.ShadowRenderType.ShadowsOnly;
		animationHelper.Target.RenderType = renderMode;
		foreach ( var clothing in animationHelper.Target.Components.GetAll<ModelRenderer>( FindMode.InChildren ) )
		{
			if ( !clothing.Tags.Has( "clothing" ) )
				continue;

			clothing.RenderType = renderMode;
		}
	}
	private void UpdateAnimation()
	{
		if ( animationHelper is null ) return;

		var wv = WishVelocity.Length;

		animationHelper.WithWishVelocity( WishVelocity );
		animationHelper.WithVelocity( controller.Velocity );
		animationHelper.IsGrounded = controller.IsOnGround;
		animationHelper.DuckLevel = Crouching ? 1.0f : 0.0f;

		animationHelper.MoveStyle = wv < 160f ? CitizenAnimationHelper.MoveStyles.Walk : CitizenAnimationHelper.MoveStyles.Run;

		var lookDir = EyeAngles.ToRotation().Forward * 1024;
		animationHelper.WithLook( lookDir, 1, 0.5f, 0.25f );
	}
	public void TakeDamage(float damage)
	{
		
		Health -= damage;
		
		Log.Info(Health);

		
}

public void AddScore(long AddScore)
{
	Score += AddScore;
}
}


