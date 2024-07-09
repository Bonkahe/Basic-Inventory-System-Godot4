using Godot;
using System;

public partial class PlayerControllerMono : CharacterBody3D
{
	[Export] public Node3D CameraContainerNode { get; set; }
	[Export] public float VerticalRotationSpeed { get; set; } = 0.01f;
    [Export] public float HorizontalRotationSpeed { get; set; } = 0.5f;


    public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

    public override void _Ready()
    {
		Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Visible ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible;
		}


		if (@event is InputEventMouseMotion motion && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			RotateY(Mathf.DegToRad(-motion.Relative.X) * HorizontalRotationSpeed);
			Vector3 cameraNodeRotation = CameraContainerNode.Rotation;
			cameraNodeRotation.X = Mathf.Clamp(cameraNodeRotation.X - motion.Relative.Y * VerticalRotationSpeed, Mathf.DegToRad(-20), Mathf.DegToRad(50));
			CameraContainerNode.Rotation = cameraNodeRotation;
		}
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		if (Input.MouseMode == Input.MouseModeEnum.Visible)
		{
            Velocity = velocity;
            MoveAndSlide();
            return;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("Left", "Right", "Forward", "Down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
