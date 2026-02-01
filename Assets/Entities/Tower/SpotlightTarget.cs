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
        speed = GameManager.Instance.alertLevel * 5 + 15;
        transform.RotateAround(center.transform.position, Vector3.up, speed * Time.deltaTime);
    }
}
