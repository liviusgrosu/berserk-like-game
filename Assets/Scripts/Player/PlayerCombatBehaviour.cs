using UnityEngine;

public class PlayerCombatBehaviour : MonoBehaviour, IEntity
{
    public LayerMask IgnoreClickMask;
    public MenuController MenuController;
    private EntityCombat _EntityCombat;
    private RaycastHit _targetRayHit;
    private Ray _targetRay;
    private PlayerEquipments _equipments;
    private EntityStats _playerStats;
    private Animator _animator;
    private PlayerMovement _movement;
    private bool _performingBlock;

    void Awake()
    {
        _EntityCombat = GetComponent<EntityCombat>();
        _equipments = GetComponent<PlayerEquipments>();
        _playerStats = GetComponent<EntityStats>();
        _animator = GetComponent<Animator>();
        _movement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // Dont perform any actions when the menu is open
        if (MenuController.IsMenuOpen())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) &&
            !_EntityCombat.IsAttacking() &&
            !_EntityCombat.IsBlocking() && 
            !_performingBlock &&
            !_movement.RollingAnimationExecuting &&
            _playerStats.CurrentStamina >= _equipments.GetCurrentWeaponStats().StaminaUse)
        {
            
            // Get ray off of what the mouse pointing to 
            _targetRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            // If a raycast collider is found then supply the target point to the attack script
            if (Physics.Raycast(_targetRay, out _targetRayHit, Mathf.Infinity, ~IgnoreClickMask))
            {
                // Execute the attack animation
                _EntityCombat.TriggerAttack(_targetRayHit.point, _equipments.GetCurrentWeaponStats().AttackSpeed);
                // Reduce stamina
                _playerStats.ReduceStamina(_equipments.GetCurrentWeaponStats().StaminaUse);
            }
        }

        if (Input.GetMouseButton(1) && 
            !_EntityCombat.IsAttacking() &&
            !_EntityCombat.IsBlocking() &&
            !_performingBlock && 
            !_movement.RollingAnimationExecuting && 
            _playerStats.CurrentStamina >= _equipments.GetCurrentWeaponStats().StaminaUse / 2.0f)
        {
            _performingBlock = true;
            //Get ray off of what the mouse pointing to 
            _targetRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Reduce current stamina
            _playerStats.ReduceStamina(_equipments.GetCurrentWeaponStats().StaminaUse / 2.0f);

            // If a raycast collider is found then supply the target point to the attack script
            if (Physics.Raycast(_targetRay, out _targetRayHit, Mathf.Infinity, ~IgnoreClickMask))
            {
                _EntityCombat.PerformBlocking(_targetRayHit.point, 1.0f);
            }
        }
        
        if(Input.GetMouseButtonUp(1))
        {
            _EntityCombat.StopBlocking();
            _performingBlock = false;
        }

        // if (Input.GetMouseButtonDown(1) && 
        //     _playerStats.CurrentStamina >= _equipments.GetCurrentWeaponStats().StaminaUse / 2.0f &&
        //     !_EntityCombat.IsAttacking() &&
        //     !_movement.RollingAnimationExecuting)
        // {
        //     // Get ray off of what the mouse pointing to 
        //     _targetRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        //     // Reduce current stamina
        //     _playerStats.ReduceStamina(_equipments.GetCurrentWeaponStats().StaminaUse / 2.0f);

        //     // If a raycast collider is found then supply the target point to the attack script
        //     if (Physics.Raycast(_targetRay, out _targetRayHit, Mathf.Infinity, ~IgnoreClickMask))
        //     {
        //         _EntityCombat.PerformBlocking(_targetRayHit.point, 1.0f);
        //     }
        // }
        // else if(Input.GetMouseButtonUp(1))
        // {
        //     // Stop the blocking animation
        //     _EntityCombat.StopBlocking();
        // }
        
    }

    public void RecieveHit(float damage)
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Blocking"))
        {
            _playerStats.ReduceStamina(_equipments.GetCurrentWeaponStats().StaminaUse);
            
            if (_playerStats.CurrentStamina < _equipments.GetCurrentWeaponStats().StaminaUse)
            {
                // Break guard
                _animator.SetTrigger("Break Guard");
                // Recieve only half damage
                _playerStats.ReduceHealth(damage / 2.0f);
            }
            else
            {
                // Block the attack and perform a block attack animation
                _playerStats.ReduceStamina(_equipments.GetCurrentWeaponStats().StaminaUse);
                _animator.SetTrigger("Block Attack");
            }
        }
        else
        {
            _playerStats.ReduceHealth(damage);
        }
    }
}