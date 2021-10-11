using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rigidbody;
    // Movement variables
    public float MovementSpeed = 5f;
    public float MovementTime = 0.8f;
    private RaycastHit mouseRay;
    // Rotation variables
    public Transform DirectionCameraOffset;
    public float TurningSpeed = 10f;
    // Rolling variables
    public float RollingSpeed = 1.5f;
    public float RollingTime = 1.0f;
    private float _currentRollingTime;
    private bool _isRolling;
    private Vector3 _rollingDirection;
    // Misc.
    public GeneratePath GridGenerator;
    private CapsuleCollider _collider;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
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
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, TurningSpeed);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }

        // --- Movement ---
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
            rigidbody.velocity = movementDirection * MovementSpeed;
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !_isRolling)
        {
            // Start the rolling countdown
            _currentRollingTime = RollingTime;
            _isRolling = true;
            // Get previous movement direction
            _rollingDirection = rigidbody.velocity / MovementSpeed;
            // Change collider
            _collider.height /= 2f;
            _collider.center -= new Vector3(0.0f, 0.5f, 0.0f);
        }
    }
}
