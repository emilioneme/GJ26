using System.Collections;
using UnityEngine;

public class RotationController : MonoBehaviour
{
    [SerializeField] float maxPitch = 80f;
    [SerializeField] float xRotationSpeed = 100f;
    [SerializeField] float yRotationSpeed = 100f;

    [SerializeField] Transform pitchTransform;

    [SerializeField] float bounceCooldown = 3f;

    float rotationX = 0f;
    float rotationY = 0f;

    float currentPitch = 0f;

    Coroutine bounceRoutine;

    private void Awake()
    {
        currentPitch = pitchTransform.localEulerAngles.x;
        if (currentPitch > 180f) currentPitch -= 360f;
    }

    private void Update()
    {
        if(bounceRoutine == null)
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

    public void Bounce() 
    {
        Quaternion oppositeRotation = transform.rotation * Quaternion.Euler(0f, 180f, 0f);
        if(bounceRoutine != null)
            StopCoroutine(bounceRoutine);
        bounceRoutine = StartCoroutine(BounceCoroutine(oppositeRotation));
    }
    IEnumerator BounceCoroutine(Quaternion rotation) 
    {
        float cooldown = 0f;
        while (cooldown < bounceCooldown) 
        {
            cooldown += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, cooldown / bounceCooldown);
            yield return null;
        }
        bounceRoutine = null;
    }

}

