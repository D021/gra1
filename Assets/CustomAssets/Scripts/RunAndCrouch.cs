using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterMotor))]
[RequireComponent (typeof (CharacterController))]
public class RunAndCrouch : MonoBehaviour
{
	private CharacterMotor _motor;
	private Transform _transform;
	private float dist;
	
	public float WalkSpeed
	{
		get { return 7; }
	}
	
	public float RunSpeed
	{
		get { return 20; }
	}
	
	public float CrouchSpeed
	{
		get { return 3; }
	}
	
	public bool RequestsRun
	{
		get { return Input.GetAxis("Run") > 0; }
	}
	
	public bool RequestsCrouch
	{
		get { return Input.GetAxis ("Crouch") > 0; }
	}
	
	public bool IsRunning
	{
		get { return RequestsRun && _motor.grounded && _motor.canControl; }
	}
	
	public bool IsCrouching
	{
		get { return RequestsCrouch && _motor.grounded && _motor.canControl; }
	}
	
	void Start () 
	{
		_motor =  GetComponent<CharacterMotor>();
		_transform = transform;
		CharacterController _controller = GetComponent<CharacterController>();
		dist = _controller.height/2;
	}
	
	void FixedUpdate ()
	{
		float vScale = 1.0f;
		float speed;
		
		if( IsRunning )
		{
			speed = RunSpeed;
		}
		else if( IsCrouching )
		{
			vScale = 0.5f;
			speed = CrouchSpeed;
		}
		else
		{
			speed = WalkSpeed;
		}
		
		_motor.movement.maxForwardSpeed = speed;
		float ultScale = _transform.localScale.y;
		
		Vector3 tmpScale = _transform.localScale;
		Vector3 tmpPosition = _transform.position;
		
		tmpScale.y = Mathf.Lerp(_transform.localScale.y, vScale, 5 * Time.deltaTime);
		_transform.localScale = tmpScale;
		
		tmpPosition.y += dist * (_transform.localScale.y - ultScale);   
		_transform.position = tmpPosition;
	}
}