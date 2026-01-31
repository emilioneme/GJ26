using UnityEngine;
using UnityEngine.Events;

public class InmateRotationInput : MonoBehaviour
{
    [SerializeField] UnityEvent<float> onSteer; // -1..1


    [SerializeField] float deadZoneDegrees = 2f;
    [SerializeField] float fullSteerDegrees = 45f;

    [SerializeField][Range(-1, 1)] float alignentMag = 1;
    [SerializeField] bool randomAlignmentMag = false;

    public Vector2 hoplessCords = Vector2.zero;

    private void Awake()
    {
        if(randomAlignmentMag)
            alignentMag = Random.Range(-1, 1f);
    }
    public void UpdateDesiredRotation(Vector2 direction)
    {
        Vector2 dir = direction;

        float alignent = alignentMag;
        //float alignentSign = Mathf.Sign(alignentMag);

        if (dir.sqrMagnitude < 0.0001f)
        {
            Vector2 from = new Vector2(transform.position.x, transform.position.z);
            dir = (hoplessCords - from).normalized;

            alignent = 1;

            //onSteer.Invoke(0f);
            //return;
        }

        Vector2 currentForward = new Vector2(transform.forward.x, transform.forward.z).normalized;
        Vector2 desired = dir.normalized;

        Debug.DrawRay(transform.position, new Vector3(currentForward.x, 0f, currentForward.y), Color.red);
        Debug.DrawRay(transform.position, new Vector3(desired.x, 0f, desired.y), Color.green);

        float signedAngle = Vector2.SignedAngle(currentForward, desired);

        /*
        if (Mathf.Abs(signedAngle) < deadZoneDegrees)
        {
            onSteer.Invoke(0f);
            return;
        }*/

        float steer = Mathf.Clamp(-signedAngle * alignent / fullSteerDegrees, -1f, 1f);

        onSteer.Invoke(steer);
    }
}
