using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAttacking : MonoBehaviour
{
    public Animator Animator;
    public float TurningSpeed = 10.0f;
    private bool _rotatingToTarget;
    private RaycastHit _targetRayHit;
    private Ray _targetRay;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Animator.GetCurrentAnimatorStateInfo(0).IsName("Blend Tree"))
        {
            Animator.SetTrigger("Attack");
            _rotatingToTarget = true;
            _targetRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        if (_rotatingToTarget)
        {
            if (Physics.Raycast(_targetRay, out _targetRayHit))
            {

                // Character will look where mouse is pointing relative to the world
                Quaternion targetRotation = Quaternion.LookRotation(_targetRayHit.point - transform.position);
                if (1.0f - Mathf.Abs(Quaternion.Dot(transform.rotation, targetRotation)) < 0.02f)
                {
                    _rotatingToTarget = false;
                    return;
                }
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, TurningSpeed);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            }
        }
    }

    public bool IsAttacking()
    {
        return Animator.GetCurrentAnimatorStateInfo(0).IsName("Attacking");
    }
}
