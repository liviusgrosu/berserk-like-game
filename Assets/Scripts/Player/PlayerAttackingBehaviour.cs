using UnityEngine;

public class PlayerAttackingBehaviour : MonoBehaviour
{
    public LayerMask IgnoreClickMask;
    private EntityAttacking _entityAttacking;
    private RaycastHit _targetRayHit;
    private Ray _targetRay;

    void Awake()
    {
        _entityAttacking = GetComponent<EntityAttacking>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Get ray off of what the mouse pointing to 
            _targetRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            // If a raycast collider is found then supply the target point to the attack script
            if (Physics.Raycast(_targetRay, out _targetRayHit, Mathf.Infinity, ~IgnoreClickMask))
            {
                _entityAttacking.TriggerAttack(_targetRayHit.point);
            }
        }
    }
}