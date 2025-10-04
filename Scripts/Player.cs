using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export] public CollisionShape3D collisionShape { get; set; }

	[Export] public Node3D Head { get; set; }

	[Export] public Camera3D Camera { get; set; }

	[Export] public RayCast3D Raycast { get; set; }

	[Export] public MeshInstance3D BlockHighlight { get; set; }

	[Export] public AnimationPlayer BreakingAnimationPlayer { get; set; }

	[Export] public Timer BreakingTimer { get; set; }

	[Export] public Control InventoryUI { get; set; }

	[Export] private float _mouseSensitivity = 0.3f;
	[Export] private float _movementSpeed = 4.35f;
	[Export] private float _jumpVelocity = 10f;

	private float _cameraXRotation;

	private float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public static Player Instance { get; private set; }

	private static bool timerFinished = false;
	public override void _Ready()
	{
		Instance = this;

		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			var mouseMotion = @event as InputEventMouseMotion;
			var deltaX = mouseMotion.Relative.Y * _mouseSensitivity;
			var deltaY = -mouseMotion.Relative.X * _mouseSensitivity;

			Head.RotateY(Mathf.DegToRad(deltaY));
			Camera.RotateX(Mathf.DegToRad(-deltaX));
			_cameraXRotation += deltaX;

			if (Head.RotationDegrees.X < -90 || Head.RotationDegrees.X > 90)
			{
				Head.RotateX(Mathf.DegToRad(90 * (Mathf.Abs(Head.RotationDegrees.X) / Head.RotationDegrees.X)));
			}
		}
	}

	private Vector3 intBlockPosition;
	private Vector3 prevBlockPos;

	private int placingCooldown = 0;
	private Vector3 collisionNormal;
	private Vector3 prevCollisionNormal;
	public override void _Process(double delta)
	{
		if (Raycast.IsColliding() && Raycast.GetCollider() is Chunk chunk && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			BlockHighlight.Visible = true;

			prevCollisionNormal = collisionNormal;
			collisionNormal = Raycast.GetCollisionNormal();

			var blockPosition = Raycast.GetCollisionPoint() - 0.5f * Raycast.GetCollisionNormal();

			prevBlockPos = intBlockPosition;

			intBlockPosition = new Vector3(Mathf.FloorToInt(blockPosition.X), Mathf.FloorToInt(blockPosition.Y), Mathf.FloorToInt(blockPosition.Z));
			BlockHighlight.GlobalPosition = intBlockPosition + new Vector3(0.5f, 0.5f, 0.5f);

			var collisionPoint = intBlockPosition + Raycast.GetCollisionNormal();

			var posDifferenceX = Raycast.GlobalPosition.X - collisionPoint.X;
			var posDifferenceY = Raycast.GlobalPosition.Y - collisionPoint.Y;
			var posDifferenceZ = Raycast.GlobalPosition.Z - collisionPoint.Z;

			var block = chunk.GetBlock((Vector3I)(intBlockPosition - chunk.GlobalPosition));

			if (Input.IsActionPressed("BreakBlock"))
			{
				if (!BreakingAnimationPlayer.IsPlaying() || prevBlockPos != intBlockPosition)
				{
					BreakingAnimationPlayer.Play("ResetTexture");
					BreakingAnimationPlayer.Play("Breaking Animation", -1, 1 / block.BreakTime);
					BreakingTimer.Start(block.BreakTime);
					timerFinished = false;
				}
				if (timerFinished == true)
				{
					chunk.SetBlock((Vector3I)(intBlockPosition - chunk.GlobalPosition), BlockManager.Instance.Air); // Mining is the same as placing
					timerFinished = false;
				}
			}

			if (Input.IsActionJustReleased("BreakBlock"))
			{
				BreakingAnimationPlayer.Play("ResetTexture");
			}

			if (IsPointInsidePlayer((BoxShape3D)collisionShape.Shape, GlobalPosition, collisionPoint))
			{
				if (Input.IsActionPressed("PlaceBlock") && collisionNormal == prevCollisionNormal)
				{
					if (placingCooldown == 0)
					{
						ChunkManager.Instance.SetBlock((Vector3I)(collisionPoint), BlockManager.Instance.OakLeaves); // Later, when we add the inventory, we can change this for the hotbar holded item
						placingCooldown = (int)Mathf.Round(Engine.GetFramesPerSecond() * 0.5);
					}
					else
					{
						placingCooldown--;
					}
				}
				else if (Input.IsActionPressed("PlaceBlock"))
				{
					placingCooldown = (int)Mathf.Round(Engine.GetFramesPerSecond() * 0.1);
				}
				else
				{
					placingCooldown = 0;
				}
			}
			
		}
		else
		{
			BlockHighlight.Visible = !(Input.MouseMode == Input.MouseModeEnum.Captured) && !InventoryUI.Visible;
			BreakingAnimationPlayer.Play("ResetTexture");
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		var velocity = Velocity;

		if (!IsOnFloor())
		{
			velocity.Y -= _gravity * (float)delta;
		}

		if (Input.IsActionPressed("Jump") && IsOnFloor() && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			velocity.Y = _jumpVelocity;
		}

		
		var inputDirection = Input.GetVector("WalkLeft", "WalkRight", "WalkBackwards", "WalkForward").Normalized();

		if (Input.MouseMode != Input.MouseModeEnum.Captured)
		{
			inputDirection = new Vector2(0, 0);
		}

		var direction = Vector3.Zero;

		direction += inputDirection.X * Head.GlobalBasis.X;

		// Forward is the negative Z direction ._.
		direction += inputDirection.Y * -Head.GlobalBasis.Z;

		velocity.X = direction.X * _movementSpeed;
		velocity.Z = direction.Z * _movementSpeed;

		Velocity = velocity;
		MoveAndSlide();
	}

	public static bool IsPointInsidePlayer(BoxShape3D player, Vector3 playerPosition, Vector3 point)
	{
		// Offset the point to capsule local space (no rotation/scaling)
		Vector3 localPoint = (point - playerPosition) + new Vector3(0.5f, 0.5f, 0.5f);

		float radius = player.Size.X;
		float halfHeight = player.Size.Y / 2f;

		return !((localPoint.X < radius && localPoint.X > -radius) && (localPoint.Y < halfHeight && localPoint.Y > -halfHeight) && (localPoint.Z < radius && localPoint.Z > -radius));
	}

	public static void OnBlockBroken()
	{
		timerFinished = true;
	}
}

