using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.XR;

public class DirectionCollider : MonoBehaviour
{
    public GameObject self;
    public UnityEvent<Vector2> averageDirection;
    public UnityEvent<Vector2> desiredDirection;

    [SerializeField] public float maxInmateDistance = 10f;
    [SerializeField] public float distanceMultiplier = 1;
    public List<GameObject> Inmates = new List<GameObject>();

    [SerializeField] float avoidanceDistance = 10;
    [SerializeField] bool isPlayer = false;
    [SerializeField] int batchID = 0;

    [SerializeField] LayerMask avoidanceLayerMask;
    [SerializeField] InmateManager inmateManager;

    [SerializeField] float alignentWeight = 1f;
    [SerializeField] float cohesionWeight = 1f;
    [SerializeField] float avoidanceWeight = 1f;


    private void Awake()
    {
        if (self == null)
            self = transform.root.gameObject; // root of this child

        if (TryGetComponent(out inmateManager))
            batchID = inmateManager.inmateBatchID;
    }

    void Update()
    {
        // Clean nulls (in case inmates despawn/disable)
        for (int i = Inmates.Count - 1; i >= 0; i--)
        {
            if (Inmates[i] == null)
                Inmates.RemoveAt(i);
        }

        if (Inmates.Count == 0)
        {
            averageDirection.Invoke(Vector2.zero);
            return;
        }

        Vector2 cumulativeDirection = Vector2.zero;
        Vector2 comulativePositon = Vector2.zero;
        Vector2 currentPositon = new Vector2(self.transform.position.x, self.transform.position.z);
        for (int i = Inmates.Count - 1; i >= 0; i--)
        {
            GameObject boid = Inmates[i];

            if (boid.TryGetComponent(out InmateManager m))
            {
                if (m.inmateBatchID != batchID && !isPlayer)
                {
                    Inmates.Remove(boid.gameObject);
                    continue;
                }
            }

            //ditsance
            float distance = Vector3.Distance(boid.transform.position, self.transform.position);
            distance = Mathf.Clamp01(distance / maxInmateDistance);
            float distanceFactor = 1 - distance;

            //alignent
            Vector2 direction = new Vector2(boid.transform.forward.x, boid.transform.forward.z);
            cumulativeDirection += direction;

            //cohesion
            Vector2 position = new Vector2(boid.transform.position.x, boid.transform.position.z);
            comulativePositon += position;
        }

        // IMPORTANT: re-check after removals
        if (Inmates.Count == 0)
        {
            averageDirection.Invoke(Vector2.zero);
            desiredDirection.Invoke(Vector2.zero);
            return;
        }

        //alignet
        Vector2 avgDirection = cumulativeDirection / Inmates.Count;
        averageDirection.Invoke(avgDirection.normalized);
        Vector2 alignentDirection = avgDirection.normalized * alignentWeight;

        //cohesion
        Vector2 avgPosition = comulativePositon / Inmates.Count;
        Vector2 posDirection = (avgPosition - currentPositon).normalized;
        Vector2 cohetionDirection = posDirection.normalized * alignentWeight;

        //avoidance
        RaycastHit raycastHit;
        Vector2 avoidanceDirection = Vector2.zero;
        if (Physics.Raycast( transform.position, transform.forward, out raycastHit, avoidanceDistance, avoidanceLayerMask))
        {
            Vector2 hitPos = new Vector2(raycastHit.point.x, raycastHit.point.z);
            avoidanceDirection = (currentPositon - hitPos).normalized;
            Debug.DrawLine(transform.position, raycastHit.point, Color.red);
            Debug.Log("Avoiding obstacle: " + raycastHit.collider.name);
            avoidanceDirection = avoidanceDirection * -1;
        }
        Vector2 avoidanceDir = avoidanceDirection.normalized * avoidanceWeight;
        Vector2 finalDirection = (alignentDirection + cohetionDirection + avoidanceDir).normalized;

        desiredDirection.Invoke(finalDirection);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Inmate") && other.gameObject != self)
        {
            if (!Inmates.Contains(other.gameObject))
                Inmates.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Inmate") && other.gameObject != self)
        {
            Inmates.Remove(other.gameObject);
        }
    }
}
