using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStateBehaviour : MonoBehaviour
{
    [Header("Distance Ranges")]
    [Tooltip("Distance to follow with the player")]
    public float EngageDistance; 
    [Tooltip("Distance to start attacking on the spot")]
    public float AttackDistance;
    [Tooltip("Distance to start calculating pathing")]
    public float PathingDistance;
    [Tooltip("Distance to start walking")]
    public float StartWalkingDistance;
    [Tooltip("Distance to despawn after death")]
    public float DespawnAfterDeathDistance;
    public float MovementSpeed = 5.0f;
    public float RunningMultiplier = 1.5f;
    private float _runningSpeed;
    private float _currentSpeed;

    [Header("Rotation")]
    // Rotation variables
    public float turnSmoothTime = 0.2f;
    private float turnSmoothVelocity;
    [HideInInspector]
    public enum States
    {
        Idle,
        Engage,
        Attack
    }
    public States CurrentState;
    [HideInInspector]
    public Transform Player;
    private NavMeshAgent _agent;
    private NavMeshPath _path;
    private Rigidbody _rigidbody;
    private Animator _animator;
    private EnemyAttackingBehaviour _attackingBehaviour;
    private EntityStats _entityStats;

    void Start()
    {
        // We use start because enemies can spawn in and awake doesnt trigger then
        CurrentState = States.Idle;
        
        // Get all relavent components
        _rigidbody = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _entityStats = GetComponent<EntityStats>();
        _attackingBehaviour = GetComponent<EnemyAttackingBehaviour>();
        _path = new NavMeshPath();
        Player = GameObject.Find("Player").transform;

        // Setup default values
        _agent.isStopped = true;
        _rigidbody.velocity = Vector3.zero;
        _runningSpeed = MovementSpeed * RunningMultiplier;
        _currentSpeed = MovementSpeed;
    }

    void Update()
    {
        // Get distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, Player.position);

        if (_entityStats.CurrentHealth <= 0.0f)
        {
            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Dying"))
            {
                // Only execute this block once upon death
                _animator.SetTrigger("Die");
                // REM: might not need to do this...
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.isKinematic = true;
                GetComponent<CapsuleCollider>().enabled = false;
                GetComponent<EntityDropSouls>().DropSouls();
                GetComponent<EntityDropLoot>().DropLoot();
                GameObject.Find("Game Manager").GetComponent<QuestManager>().TriggerEvent(QuestObjective.Type.Kill, name);
                _agent.enabled = false;
            }

            if (distanceToPlayer >= DespawnAfterDeathDistance)
            {
                Destroy(this.gameObject);
            }

            return;
        }

        // Don't start calculating the pathing unless they're relatively close
        if (distanceToPlayer <= PathingDistance)
        {
            _agent.destination = Player.position;
        }

        if (CurrentState == States.Idle)
        {   
            // Speed
            _rigidbody.velocity = Vector3.zero;

            // Engage with the player if they get close
            if  (distanceToPlayer <= EngageDistance)
            {
                CurrentState = States.Engage;
                _agent.isStopped = false;
            }
        }
        else if (CurrentState == States.Engage)
        {
            // Speed
            // Run to the player
            if (distanceToPlayer > StartWalkingDistance)
            {
                _currentSpeed = _runningSpeed;
            }
            // walk to the player
            else
            {
                _currentSpeed = MovementSpeed;
            }

            Vector3 normalizedAgentVelocity = _agent.desiredVelocity.normalized; 
            _rigidbody.velocity = normalizedAgentVelocity * _currentSpeed;

            // Rotate to the next goal point
            RotateToTarget(normalizedAgentVelocity);
            // Go back to idle if player is too far away
            if  (distanceToPlayer > EngageDistance)
            {
                CurrentState = States.Idle;
                _agent.isStopped = true;
            }

            // Start attacking the player if they're close
            if (distanceToPlayer <= AttackDistance)
            {
                CurrentState = States.Attack;
                _agent.isStopped = true;
            }
        }
        else if (CurrentState == States.Attack)
        {
            // Speed
            Vector3 normalizedAgentVelocity = Player.position - transform.position;
            _rigidbody.velocity = Vector3.zero;

            // Rotate to the next goal point
            RotateToTarget(normalizedAgentVelocity);
            
            // Pursuit the player if they are far enough
            if (distanceToPlayer > AttackDistance && !_animator.GetCurrentAnimatorStateInfo(0).IsName("Attacking"))
            {
                CurrentState = States.Engage;
                _agent.isStopped = false;
            }
        }

        _animator.SetFloat("speedPercent", _rigidbody.velocity.magnitude / _runningSpeed);
    }

    private void RotateToTarget(Vector3 normalizedAgentVelocity)
    {
        // Rotate to the next goal point
        float targetRotation = Mathf.Atan2(normalizedAgentVelocity.x, normalizedAgentVelocity.z) * Mathf.Rad2Deg;
        transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
    }
}
