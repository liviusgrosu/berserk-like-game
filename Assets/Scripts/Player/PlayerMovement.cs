using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rigidbody;
    public Transform DirectionCameraOffset;
    public float MovementSpeed = 5f;
    public float TurningSpeed = 10f;
    
    private RaycastHit mouseRay;

    public GeneratePath GridGenerator;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // transform.position = GridGenerator.GetStartRoomPosition() + new Vector3(0f, 1.5f, 0f);
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            // Character will look where mouse is pointing relative to the world
            Quaternion targetRotation = Quaternion.LookRotation(hit.point - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, TurningSpeed);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }

        Vector3 movementDirection = Vector3.zero;

        // Move character relative to the camera rotation offset
        movementDirection += DirectionCameraOffset.forward * Input.GetAxisRaw("Vertical");
        movementDirection += DirectionCameraOffset.right * Input.GetAxisRaw("Horizontal");

        // Eliminate double speed with multiple inputs
        movementDirection = movementDirection.normalized;
        rigidbody.velocity = movementDirection * MovementSpeed;
    }
}
