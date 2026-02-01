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

    [Header("Weights")]
    [SerializeField] float alignmentWeight = 1f;
    [SerializeField] float avoidanceWeight = 1f;

    [Header("Settings")]
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] public float maxAvoidanceDist = 5f;
    [SerializeField] public float maxInmateDistance = 10f;
    [SerializeField] public float distanceMultiplier = 1;
    public List<GameObject> Inmates = new List<GameObject>();

    //[SerializeField] float idleInfleunce = 0
    [SerializeField] bool idleInfluence = false;

    private void Awake()
    {
        if (self == null)
            self = transform.root.gameObject; // root of this child
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
        for (int i = Inmates.Count - 1; i >= 0; i--)
        {
            GameObject boid = Inmates[i];

            float distance = Vector3.Distance(boid.transform.position, self.transform.position);
            distance = Mathf.Clamp01(distance / maxInmateDistance);
            float distanceFactor = 1 - distance;

            //bool isIdle = false;
            if (boid.TryGetComponent(out MovingController mc))
            {
                if (mc.moveInput.magnitude < 0.1f && !idleInfluence)
                {
                    Inmates.Remove(boid.gameObject);
                    continue;
                }
            }

            Vector2 direction = new Vector2(boid.transform.forward.x, boid.transform.forward.z);
            cumulativeDirection += direction * distanceFactor;
        }

        Vector2 avgDirection = cumulativeDirection / Inmates.Count;
        averageDirection.Invoke(avgDirection.normalized);
        Vector2 alignentDirection = avgDirection * alignmentWeight;
        Debug.DrawRay(self.transform.position, new Vector3(alignentDirection.x, 0, alignentDirection.y), Color.green);

        //Avoidance
        RaycastHit hit;
        Vector3 avoidanceDirection3 = Vector3.zero;
        Vector2 avoidanceDirection = Vector3.zero;
        if (Physics.Raycast(self.transform.position, self.transform.position, out hit, maxAvoidanceDist, obstacleLayer)) 
        {
            avoidanceDirection = -(self.transform.position - hit.point).normalized;
        }
        if(hit.collider != null)
            Debug.DrawRay(self.transform.position, avoidanceDirection * maxAvoidanceDist, Color.red);
        else
            Debug.DrawRay(self.transform.position, self.transform.forward * maxAvoidanceDist, Color.red);
        avoidanceDirection = new Vector2(avoidanceDirection3.x, avoidanceDirection3.z) * avoidanceWeight;


        Vector2 finalDirection = (alignentDirection + avoidanceDirection).normalized;
        desiredDirection.Invoke((finalDirection).normalized);
        Debug.DrawRay(self.transform.position, new Vector3(finalDirection.x, 0, finalDirection.y) * 5f, Color.blue);

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

