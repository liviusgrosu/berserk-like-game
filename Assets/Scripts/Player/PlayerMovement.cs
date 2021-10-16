using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    // Movement variables
    public float MovementSpeed = 5.0f;
    public float RunningMultiplier = 1.5f;
    public float RunningStaminaCost = 0.1f;
    private float _runningSpeed;
    private float _currentSpeed;
    private bool _isRunning;
    [Header("Rotation")]
    // Rotation variables
    public float TurningSpeed = 10.0f;
    public float turnSmoothTime = 0.2f;
    private float turnSmoothVelocity;
    private RaycastHit mouseRay;
    // Rolling variables
    [Header("Rolling")]
    public float RollingSpeed = 1.5f;
    public float RollingTime = 1.0f;
    public float RollingStaminaCost = 2.0f;
    private float _currentRollingTime;
    private bool _isRolling;
    private Vector3 _rollingDirection;
    // Misc.
    [Header("Components")]
    public GeneratePath GridGenerator;
    public EntityStats PlayerStats;
    public Animator Animator;
    private CapsuleCollider _collider;
    public Transform DirectionCameraOffset;
    private Rigidbody rigidbody;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();

        _runningSpeed = MovementSpeed * RunningMultiplier;
        _currentSpeed = MovementSpeed;
    }

    void Start()
    {
        // transform.position = GridGenerator.GetStartRoomPosition() + new Vector3(0f, 1.5f, 0f);
    }

    void Update()
    {
        HandleInput();

        // --- Rotation ---
        RaycastHit hit; 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            // Character will look where mouse is pointing relative to the world
            Quaternion targetRotation = Quaternion.LookRotation(hit.point - transform.position);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, TurningSpeed);
            //transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }

        // --- Movement ---
        if (_isRunning)
        {
            PlayerStats.ReduceStamina(RunningStaminaCost * Time.deltaTime);
            // Stop running if player has ran out of stamina
            if (PlayerStats.CurrentStamina <= 0.0f)
            {
                _currentSpeed = MovementSpeed;
                _isRunning = false;
            }
        }
        
        if (_isRolling)
        {
            rigidbody.velocity = _rollingDirection * RollingSpeed;
            _currentRollingTime -= Time.deltaTime;
            if (_currentRollingTime <= 0.0f)
            {
                _isRolling = false;
                // Change collider
                _collider.center += new Vector3(0.0f, 0.5f, 0.0f);
                _collider.height *= 2f;
                // Rest movement back regular speed
                _currentSpeed = MovementSpeed;
                _isRunning = false;
            }
        }
        else
        {
            Vector3 movementDirection = Vector3.zero;

            // Move character relative to the camera rotation offset
            movementDirection += DirectionCameraOffset.forward * Input.GetAxisRaw("Vertical");
            movementDirection += DirectionCameraOffset.right * Input.GetAxisRaw("Horizontal");

            // Eliminate double speed with multiple inputs
            movementDirection = movementDirection.normalized;

            // Rotate the player based on their movement
            if (movementDirection != Vector3.zero)
            {
                float targetRotation = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg;
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
            }

            rigidbody.velocity = movementDirection * _currentSpeed;
        }

        Animator.SetFloat("speedPercent", rigidbody.velocity.magnitude / MovementSpeed);
    }

    void HandleInput()
    {
        // Handle rolling
        if (Input.GetKeyDown(KeyCode.Space) && 
            !_isRolling && 
            PlayerStats.CurrentStamina >= RollingStaminaCost &&
            Vector3.Magnitude(rigidbody.velocity) > 0.0f)
        {
            // Start the rolling countdown
            _currentRollingTime = RollingTime;
            _isRolling = true;
            // Get previous movement direction
            _rollingDirection = rigidbody.velocity / _currentSpeed;
            // Change collider
            _collider.height /= 2.0f;
            _collider.center -= new Vector3(0.0f, 0.5f, 0.0f);
            // Reduce stamina cost
            PlayerStats.ReduceStamina(RollingStaminaCost);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _currentSpeed = _runningSpeed;
            _isRunning = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _currentSpeed = MovementSpeed;
            _isRunning = false;
        }
    }
}
