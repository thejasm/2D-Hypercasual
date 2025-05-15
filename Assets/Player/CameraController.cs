using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10);
    void Start()
    {
        if (target == null) target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        this.transform.position = target.position + offset;
    }
}
