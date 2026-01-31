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

    [SerializeField] bool debugAlignent = false;
    [SerializeField] bool debugCohesiveness = false;
    [SerializeField] bool debugAvoidance = false;

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
        }

        Vector2 cumulativeDirection = Vector2.zero;

        Vector2 currentPositon = new Vector2(self.transform.position.x, self.transform.position.z);
        Vector3 currentPositon3 = new Vector3(self.transform.position.x, self.transform.position.y, self.transform.position.z);

        Vector2 comulativePositon = Vector2.zero;

        for (int i = Inmates.Count - 1; i >= 0; i--)
        {
            GameObject boid = Inmates[i];

            // distance
            float distance = Vector3.Distance(boid.transform.position, self.transform.position);
            distance = Mathf.Clamp01(distance / maxInmateDistance);
            float distanceFactor = 1f - distance;

            // alignment (optionally weighted by distanceFactor)
            Vector2 direction = new Vector2(boid.transform.forward.x, boid.transform.forward.z);
            cumulativeDirection += direction * distanceFactor;

            // cohesion (optionally weighted by distanceFactor)
            Vector2 position = new Vector2(boid.transform.position.x, boid.transform.position.z);
            comulativePositon += position * distanceFactor;
        }

        // avoidance
        RaycastHit raycastHit;
        Vector2 avoidanceDirection = Vector2.zero;

        //Debug.DrawRay(currentPositon3, self.transform.forward * avoidanceDistance, Color.red);
        if (Physics.Raycast(currentPositon3, self.transform.forward, out raycastHit, avoidanceDistance, avoidanceLayerMask))
        {
            Vector2 hitPos = new Vector2(raycastHit.point.x, raycastHit.point.z);

            // steer AWAY from obstacle
            avoidanceDirection = (currentPositon - hitPos).normalized;
        }

        Vector2 avoidanceDir = avoidanceDirection * avoidanceWeight;

        if (debugAvoidance)
        {
            //Debug.DrawRay(currentPositon3, new Vector3(avoidanceDir.x, 0f, avoidanceDir.y), Color.yellow);
        }
            

        Vector2 alignentDirection = Vector2.zero;
        Vector2 cohetionDirection = Vector2.zero;

        // IMPORTANT: re-check after removals
        if (Inmates.Count == 0)
        {
            averageDirection.Invoke(Vector2.zero);
        } 
        else
        {
           // alignment
            Vector2 avgDirection = cumulativeDirection / Inmates.Count;
            averageDirection.Invoke(avgDirection.normalized);

            alignentDirection = avgDirection.normalized * alignentWeight;
            if (debugAlignent)
                Debug.DrawRay(currentPositon3, new Vector3(alignentDirection.x, 0f, alignentDirection.y), Color.yellow);

            // cohesion
            // Vector2 avgPosition = comulativePositon / Inmates.Count;
            // Vector2 posDirection = (avgPosition - currentPositon).normalized;

            // // If you *do* have a cohesionWeight, use it here; otherwise keep alignentWeight
            // cohetionDirection = posDirection * alignentWeight;

            // if (debugCohesiveness)
            //     Debug.DrawRay(currentPositon3, new Vector3(cohetionDirection.x, 0f, cohetionDirection.y), Color.green);
        }

        
        // final
        Vector2 finalDirection = (-alignentDirection + cohetionDirection + avoidanceDir).normalized;
        //Debug.DrawRay(currentPositon3, new Vector3(finalDirection.x, 0f, finalDirection.y), Color.blue);

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
