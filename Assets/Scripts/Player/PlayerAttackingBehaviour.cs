using UnityEngine;

public class PlayerAttackingBehaviour : MonoBehaviour
{
    public LayerMask IgnoreClickMask;
    public MenuController MenuController;
    private EntityAttacking _entityAttacking;
    private RaycastHit _targetRayHit;
    private Ray _targetRay;
    private IEquipment _equipments;
    private EntityStats _playerStats;
    private Animator _animator;
    private PlayerMovement _movement;


    void Awake()
    {
        _entityAttacking = GetComponent<EntityAttacking>();
        _equipments = GetComponent<IEquipment>();
        _playerStats = GetComponent<EntityStats>();
        _animator = GetComponent<Animator>();
        _movement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (MenuController.IsMenuOpen())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) &&
            !_entityAttacking.IsAttacking() && 
            !_entityAttacking.IsBlocking() &&
            !_movement.IsRolling())
        {
            
            // Get ray off of what the mouse pointing to 
            _targetRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            // If a raycast collider is found then supply the target point to the attack script
            if (Physics.Raycast(_targetRay, out _targetRayHit, Mathf.Infinity, ~IgnoreClickMask))
            {
                _entityAttacking.TriggerAttack(_targetRayHit.point, ((WeaponStats)_equipments.GetCurrentEquipmentStats()).AttackSpeed);
                _playerStats.ReduceStamina(((WeaponStats)_equipments.GetCurrentEquipmentStats()).StaminaUse);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            // Get ray off of what the mouse pointing to 
            _targetRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            // If a raycast collider is found then supply the target point to the attack script
            if (Physics.Raycast(_targetRay, out _targetRayHit, Mathf.Infinity, ~IgnoreClickMask))
            {
                _entityAttacking.PerformBlocking(_targetRayHit.point, 1.0f);
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            // Stop the blocking animation
            _entityAttacking.StopBlocking();
        }
    }
}