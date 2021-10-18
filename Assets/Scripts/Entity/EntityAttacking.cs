using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAttacking : MonoBehaviour
{
    public Animator Animator;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Animator.GetCurrentAnimatorStateInfo(0).IsName("Blend Tree"))
        {
            Animator.SetTrigger("Attack");
        }
    }

    public bool IsAttacking()
    {
        return Animator.GetCurrentAnimatorStateInfo(0).IsName("Attacking");
    }
}
