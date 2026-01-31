using System;
using UnityEngine;

public class SpotlightTarget : MonoBehaviour
{
    [SerializeField]
    public GameObject center;

    [SerializeField]
    public float speed = 10f;

    void Update()
    {
        transform.RotateAround(center.transform.position, Vector3.up, speed * Time.deltaTime);
    }
}
