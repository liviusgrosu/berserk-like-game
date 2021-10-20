using UnityEngine;

public class EntityAttacking : MonoBehaviour
{
    public Animator Animator;
    public float TurningSpeed = 10.0f;
    private bool _rotatingToTarget;
    private Vector3 _targetLookDirection;

    // Update is called once per frame
    void Update()
    {
        // Rotate to the target
        if (_rotatingToTarget)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_targetLookDirection - transform.position);
            if (1.0f - Mathf.Abs(Quaternion.Dot(transform.rotation, targetRotation)) < 0.02f)
            {
                _rotatingToTarget = false;
                return;
            }
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, TurningSpeed);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }

    public void TriggerAttack(Vector3 targetPosition)
    {
        // Only trigger attack when entity is not already attacking
        if (Animator.GetCurrentAnimatorStateInfo(0).IsName("Blend Tree"))
        {
            Animator.SetTrigger("Attack");
            _targetLookDirection = targetPosition;
            _rotatingToTarget = true;
        }
    }

    public bool IsAttacking()
    {
        return Animator.GetCurrentAnimatorStateInfo(0).IsName("Attacking");
    }
}
