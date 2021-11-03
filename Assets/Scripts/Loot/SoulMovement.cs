using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulMovement : MonoBehaviour
{
    public int SoulAmount;
    private Transform _player;
    // Movement
    private float _speed = 1.0f;
    public float SpeedMultiplier = 2.0f;
    public float TopSpeed = 10.0f;
    public float startMovementRange = 10.0f;
    private bool isMoving;
    // Floating effect
    private Vector3 pointA, pointB;
    private float step, time;
    void Start()
    {
        _player = GameObject.Find("Player Centre").transform;

        pointA = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
        pointB = new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z);
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, _player.position) <= startMovementRange)
        {
            // Start moving when the player is in range
            isMoving = true;
        }

        if (isMoving)
        {
            // Increase the speed towards the player until it reaches a cap
            float step = _speed * Time.deltaTime;
            if (_speed <= TopSpeed)
            {
                _speed += Time.deltaTime * SpeedMultiplier;
            }

            // Move to the player
            transform.position = Vector3.MoveTowards(transform.position, _player.position, step);

            if (Vector3.Distance(transform.position, _player.position) <= 0.01f)
            {
                // Add to players soul count
                GameObject.Find("Game Manager").GetComponent<LootManager>().SoulCount += SoulAmount;
                Destroy(this.gameObject);
            }
        }
        else
        {
            // Bounce up and down
            time += Time.deltaTime / 2.0f;
            step = time / 2.0f;
            step = step * step * (3f - 2f * step);

            if (step >= 0.99f)
            {
                Vector3 pointTemp = pointA;
                pointA = pointB;
                pointB = pointTemp;
                step = 0.0f;
                time = 0.0f;
            }

            transform.position = Vector3.Slerp(pointA, pointB, step);
        }
    }
}
