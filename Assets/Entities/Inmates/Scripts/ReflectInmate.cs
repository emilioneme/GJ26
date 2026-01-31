using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CharacterController))]
public class ReflectInmate : MonoBehaviour
{
    [SerializeField] float randomBounceAngle = 15f;
    CharacterController cc;

    public UnityEvent onBounce;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("HIT: " + hit.collider.name); // put this FIRST for debugging

        if (hit.collider.CompareTag("Floor"))
            return;

        Debug.Log($"{name} bounced off {hit.collider.name}");
        onBounce.Invoke();
    }
}