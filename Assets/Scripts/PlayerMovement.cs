using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rigidbody;
    public Transform DirectionCameraOffset;
    public float MovementSpeed;
    
    private RaycastHit mouseRay;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            Quaternion targetRotation = Quaternion.LookRotation(hit.point - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 100.0f);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }

        Vector3 movementDirection = Vector3.zero;
        
        if (Input.GetKey(KeyCode.W))
        {
            movementDirection += DirectionCameraOffset.forward;
        }

        if (Input.GetKey(KeyCode.S))
        {
            movementDirection -= DirectionCameraOffset.forward;
        }

        if (Input.GetKey(KeyCode.D))
        {
            movementDirection += DirectionCameraOffset.right;
        }

        if (Input.GetKey(KeyCode.A))
        {
            movementDirection -= DirectionCameraOffset.right;
        }

        rigidbody.velocity = movementDirection * MovementSpeed;
    }
}
