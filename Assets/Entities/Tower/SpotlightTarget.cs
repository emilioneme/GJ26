using System;
using UnityEngine;

public class SpotlightTarget : MonoBehaviour
{
    [SerializeField]
    public GameObject center;

    [SerializeField]
    public float speed = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(center.transform.position, Vector3.up, speed * Time.deltaTime);
    }
}
