using UnityEngine;
using UnityEngine.Events;

public class InmateRotationInput : MonoBehaviour
{
    [SerializeField] UnityEvent<float> onSteer; // -1..1


    [SerializeField] float deadZoneDegrees = 2f;


    [SerializeField][Range(-.4f, 1)] float alignentRandomness = 1;
    [SerializeField][Range(-1, 1)] float alignentMag = 1;

    [SerializeField] float fullSteerDegrees = 45f;
    [SerializeField] float steerSmoothTime = 0.25f;  // bigger = slower, smoother
    [SerializeField] float maxSteerSpeed = 10f;      // limits how fast it can change

    [SerializeField] bool randomAtStart = false;

    float randomSteer => Random.Range(-1f, 1f);
    float currentSteer = 0f;
    float steerVelocity = 0f;

    void Start()
    {
        if(randomAtStart)
            ChangeBehaviour();
    }

    public void UpdateDesiredRotation(Vector2 direction) //avrg direction
    {
        Vector2 dir = direction;

        Vector2 currentForward = new Vector2(transform.forward.x, transform.forward.z).normalized;
        Vector2 desired = dir.normalized;

        Debug.DrawRay(transform.position, new Vector3(currentForward.x, 0f, currentForward.y), Color.red);
        Debug.DrawRay(transform.position, new Vector3(desired.x, 0f, desired.y), Color.green);

        float signedAngle = Vector2.SignedAngle(currentForward, desired);
        signedAngle = Mathf.Clamp(signedAngle, -fullSteerDegrees, fullSteerDegrees);
        float desiredSteer;

        if (Mathf.Abs(signedAngle) < deadZoneDegrees)
            desiredSteer = randomSteer; 
        else
        desiredSteer = Mathf.Clamp(-signedAngle / fullSteerDegrees, -1, 1);
            desiredSteer *= alignentMag;

        currentSteer = Mathf.SmoothDamp(
            currentSteer,
            desiredSteer,
            ref steerVelocity,
            steerSmoothTime,
            maxSteerSpeed,
            Time.deltaTime
        );

        onSteer.Invoke(currentSteer);
    }

    public void ChangeBehaviour() 
    {
        alignentMag = Random.Range(alignentRandomness, 1);
        //randomSteer = Random.Range(-1f, 1f);
    }
}
