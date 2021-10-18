using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAttacking : MonoBehaviour
{
    public Animator Animator;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Animator.SetTrigger("Attack");
        }
    }
}
