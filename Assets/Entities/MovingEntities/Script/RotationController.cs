using System.Collections;
using UnityEngine;

public class RotationController : MonoBehaviour
{
    [SerializeField] float maxPitch = 80f;
    [SerializeField] float xRotationSpeed = 100f;
    [SerializeField] float yRotationSpeed = 100f;

    [SerializeField] Transform pitchTransform;

    float rotationX = 0f;
    float rotationY = 0f;

    float currentPitch = 0f;

    private void Awake()
    {
        currentPitch = pitchTransform.localEulerAngles.x;
        if (currentPitch > 180f) currentPitch -= 360f;
    }

    private void Update()
    {

        transform.Rotate(Vector3.up, rotationY * yRotationSpeed * Time.deltaTime, Space.Self);

        currentPitch -= rotationX * xRotationSpeed * Time.deltaTime;
        currentPitch = Mathf.Clamp(currentPitch, -maxPitch, maxPitch);

        pitchTransform.localRotation = Quaternion.Euler(currentPitch, 0f, 0f);
    }

    public void UpdateVerticleInput(float input)
    {
        rotationX = input;
    }

    public void UpdateHorizontalInput(float input)
    {
        rotationY = input;
    }

}

