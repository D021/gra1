#pragma strict

public var walkSpeed: float = 7;
public var crouchSpeed: float = 3;
public var runSpeed: float = 20;
public var jumpHeight: float = 1;

function Start() { }

function FixedUpdate()
{
	if( IsGrounded() )
	{
		Movement();
	}
}

function Movement()
{
	var vec = Vector3.zero;
	var speed = walkSpeed;
	if( Input.GetAxis("Horizontal") || Input.GetAxis("Vertical") )
	{
		vec = Vector3(Input.GetAxisRaw("Horizontal"),vec.y, Input.GetAxisRaw("Vertical"));
		this.transform.Translate(vec.normalized * speed * Time.deltaTime);
	}
}

/*
function CrouchWalk()
{
	var vScale = transform.localScale.y;
	var speed = walkSpeed;
	
	if( Input.GetKey("left shift") || Input.GetKey("right shift") )
	{
	    speed = runSpeed;
	}
	if (Input.GetKey("c")){ // press C to crouch
	    vScale = 0.5;
	    speed = crchSpeed; // slow down when crouching
	}
	chMotor.movement.maxForwardSpeed = speed; // set max speed
	var ultScale = tr.localScale.y; // crouch/stand up smoothly 
	tr.localScale.y = Mathf.Lerp(tr.localScale.y, vScale, 5*Time.deltaTime);
	tr.position.y += dist * (tr.localScale.y-ultScale); // fix vertical position
}*/

function IsGrounded(): boolean
{
	var distToGround = collider.bounds.extents.y;
	return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1);
}