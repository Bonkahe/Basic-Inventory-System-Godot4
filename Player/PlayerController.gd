extends CharacterBody3D

@export var CameraContainerNode : Node3D
@export var VerticalRotationSpeed : float = 0.01
@export var HorizontalRotationSpeed : float = 0.5

const SPEED = 5.0
const JUMP_VELOCITY = 4.5

func _ready() -> void:
	Input.mouse_mode = Input.MOUSE_MODE_CAPTURED

func _input(event: InputEvent) -> void:
	if (event.is_action_pressed("ui_cancel")):
		Input.mouse_mode = Input.MOUSE_MODE_CAPTURED if (Input.mouse_mode == Input.MOUSE_MODE_VISIBLE) else Input.MOUSE_MODE_VISIBLE
	
	if (event is InputEventMouseMotion and Input.mouse_mode == Input.MOUSE_MODE_CAPTURED):
		rotate_y(deg_to_rad(-event.relative.x) * HorizontalRotationSpeed)
		CameraContainerNode.rotation.x = clamp(CameraContainerNode.rotation.x - event.relative.y * VerticalRotationSpeed, deg_to_rad(-20), deg_to_rad(50))


func _physics_process(delta: float) -> void:
	# Add the gravity.
	if not is_on_floor():
		velocity += get_gravity() * delta
	
	if (Input.mouse_mode == Input.MOUSE_MODE_VISIBLE):
		move_and_slide()
		return
	
	# Handle jump.
	if Input.is_action_just_pressed("ui_accept") and is_on_floor():
		velocity.y = JUMP_VELOCITY

	# Get the input direction and handle the movement/deceleration.
	# As good practice, you should replace UI actions with custom gameplay actions.
	var input_dir := Input.get_vector("Left", "Right", "Forward", "Down")
	var direction := (transform.basis * Vector3(input_dir.x, 0, input_dir.y)).normalized()
	if direction:
		velocity.x = direction.x * SPEED
		velocity.z = direction.z * SPEED
	else:
		velocity.x = move_toward(velocity.x, 0, SPEED)
		velocity.z = move_toward(velocity.z, 0, SPEED)

	move_and_slide()
