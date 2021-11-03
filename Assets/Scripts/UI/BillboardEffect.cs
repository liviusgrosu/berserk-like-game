using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardEffect : MonoBehaviour
{
    public enum FaceDirection
    {
        forward,
        up
    };
    public FaceDirection direction;
    void LateUpdate()
    {
        Vector3 targetDirection = Vector3.zero;
        switch(direction)
        {
            case FaceDirection.forward:
                targetDirection = Camera.main.transform.forward;
                break;
            case FaceDirection.up:
                targetDirection = Camera.main.transform.up;
                break;
        }

        transform.LookAt(transform.position + targetDirection);
    }
}
