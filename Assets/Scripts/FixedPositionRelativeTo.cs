using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedPositionRelativeTo : MonoBehaviour
{
    public Transform AnchorObject;

    private Vector3 offset;

    void Start()
    {
        offset = transform.position - AnchorObject.position;
    }

    void Update()
    {
        transform.position = AnchorObject.position + offset;
    }
}
