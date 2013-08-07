using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterMotor))]
[RequireComponent (typeof (CharacterController))]
[RequireComponent (typeof (Animator))]
public class RunAndCrouch : MonoBehaviour
{
	private CharacterMotor _motor;
	private Transform _transform;
	private float _dist;
	private float _maxForwardSpeed;
	private Animator _animator;
    
    private int _speedId;
    private int _agularSpeedId;
    private int _directionId;
	
	public float WalkSpeed
	{
		//TODO: forward, backwards, sideways distinction
		get { return _maxForwardSpeed; }
	}
	
	public float RunSpeed
	{
		get { return WalkSpeed * 2; }
	}
	
	public float CrouchSpeed
	{
		get { return WalkSpeed / 2; }
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
	
	void Start() 
	{
		_motor =  GetComponent<CharacterMotor>();
		_transform = transform;
		CharacterController _controller = GetComponent<CharacterController>();
		_dist = _controller.height/2;
		_maxForwardSpeed = _motor.movement.maxForwardSpeed;
		_animator = GetComponent<Animator>();

        _speedId = Animator.StringToHash("Speed");
        _agularSpeedId = Animator.StringToHash("AngularSpeed");
        _directionId = Animator.StringToHash("Direction");
	}
	
	void FixedUpdate()
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
		Animate();
		
		TransformCrouch(vScale);
	}
	
	void TransformCrouch(float vScale)
	{
		float ultScale = _transform.localScale.y;
		
		Vector3 tmpScale = _transform.localScale;
		Vector3 tmpPosition = _transform.position;
		
		tmpScale.y = Mathf.Lerp(_transform.localScale.y, vScale, 5 * Time.deltaTime);
		_transform.localScale = tmpScale;
		
		tmpPosition.y += _dist * (_transform.localScale.y - ultScale);   
		_transform.position = tmpPosition;
	}
	
	void Animate()
	{
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float speed = (h * h + v * v) * 6;
        float direction = Mathf.Atan2(h, v) * 180.0f / 3.14159f;
		
        AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(0);

        bool inTransition = _animator.IsInTransition(0);
        bool inIdle = state.IsName("Locomotion.Idle");
        bool inTurn = state.IsName("Locomotion.TurnOnSpot") || state.IsName("Locomotion.PlantNTurnLeft") || state.IsName("Locomotion.PlantNTurnRight");
        bool inWalkRun = state.IsName("Locomotion.WalkRun");

        float speedDampTime = inIdle ? 0 : 0.1f;
        float angularSpeedDampTime = inWalkRun || inTransition ? 0.25f : 0;
        float directionDampTime = inTurn || inTransition ? 1000000 : 0;

        float angularSpeed = direction / 0.2f;
        
        _animator.SetFloat(_speedId, speed, speedDampTime, Time.deltaTime);
        _animator.SetFloat(_agularSpeedId, angularSpeed, angularSpeedDampTime, Time.deltaTime);
        _animator.SetFloat(_directionId, direction, directionDampTime, Time.deltaTime);
	}
}