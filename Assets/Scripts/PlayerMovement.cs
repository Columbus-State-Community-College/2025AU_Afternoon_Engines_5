using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")] 
    public float walkSpeed;
    public float sprintSpeed;

    public float groundDrag;
    public float airMultiplier;

    public float jumpForce;

    public float gravity;

    [Header("Ground Check")] 
    public float playerHeight;
    
    [Header("Slope Handling")]
    public float maxSlopeAngle;
    
    private Transform _orientation;

    private float _horizontalInput;
    private float _verticalInput;

    private float _moveSpeed;
    private Vector3 _moveDirection;

    private Rigidbody _rb;
    
    private bool _grounded;
    private bool _canJump;

    private ConstantForce _gravityForce;

    private MovementState _state;

    private RaycastHit _slopeHit;
    private bool _exitingSlope;

    private enum MovementState
    {
        Walking,
        Sprinting,
        Airborne
    }

    private void Start()
    {
        _orientation = GameObject.Find("Player").transform.Find("Orientation").transform;
        _rb = GetComponent<Rigidbody>();
        _gravityForce = gameObject.AddComponent<ConstantForce>();
        
        _rb.freezeRotation = true;
        _rb.useGravity = false;
        _gravityForce.force = new Vector3(0f, gravity, 0f);
    }

    private void Update()
    {
        _grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f);
        
        GetInput();
        ControlSpeed();
        StateHandler();

        if (!_grounded) return;

        _rb.linearDamping = groundDrag;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void GetInput()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump") && _grounded) { Jump(); }
    }

    private void MovePlayer()
    {
        _moveDirection = (_orientation.forward * _verticalInput) + (_orientation.right * _horizontalInput);

        if (OnSlope() && !_exitingSlope)
        {
            _rb.AddForce(20f * _moveSpeed * GetSlopeMovementDirection(), ForceMode.Force);

            if (_rb.linearVelocity.y > 0)
            {
                _rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
            
            _gravityForce.force = Vector3.zero;
        }
        else
        {
            _exitingSlope = true;
            _gravityForce.force = new Vector3(0f, gravity, 0f);
        }
        
        if (_grounded)
        {
            _rb.AddForce(_moveSpeed * 10f * _moveDirection.normalized, ForceMode.Force);
        }
        else
        {
            _rb.AddForce(_moveSpeed * 10f * airMultiplier * _moveDirection.normalized, ForceMode.Force);
        }
    }

    private void ControlSpeed()
    {
        if (OnSlope())
        {
            if (_rb.linearVelocity.magnitude > _moveSpeed)
            {
                _rb.linearVelocity = _rb.linearVelocity.normalized * _moveSpeed;
            }
        }
        else
        {
            Vector3 flatVelocity = new(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);

            if (flatVelocity.magnitude <= _moveSpeed) return;
        
            var limitedVelocity = flatVelocity.normalized * _moveSpeed;
            _rb.linearVelocity = new Vector3(limitedVelocity.x, _rb.linearVelocity.y, limitedVelocity.z);
        }
    }

    private void Jump()
    {
        _exitingSlope = true;
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
        
        _rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void StateHandler()
    {
        switch (_grounded)
        {
            case true when Input.GetButton("Sprint"):
                _state = MovementState.Sprinting;
                _moveSpeed = sprintSpeed;
                break;
            case true:
                _state = MovementState.Walking;
                _moveSpeed = walkSpeed;
                break;
            default:
                _state = MovementState.Airborne;
                break;
        }
    }

    private bool OnSlope()
    {
        if (!Physics.Raycast(transform.position, Vector3.down, out _slopeHit, playerHeight * 0.5f + 0.3f)) return false;
        
        var angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
        return angle < maxSlopeAngle && angle != 0;
    }

    private Vector3 GetSlopeMovementDirection()
    {
        return Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal).normalized;
    }
}
