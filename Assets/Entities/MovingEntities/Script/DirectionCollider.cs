using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.XR;

public class DirectionCollider : MonoBehaviour
{
    public GameObject self;
    public UnityEvent<Vector2> averageDirection;

    [SerializeField] public float maxInmateDistance = 10f;
    [SerializeField] public float distanceMultiplier = 1;
    public List<GameObject> Inmates = new List<GameObject>();

    [SerializeField] bool isPlayer = false;
    [SerializeField] int batchID = 0;

    [SerializeField] InmateManager inmateManager;


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

            float distance = Vector3.Distance(boid.transform.position, self.transform.position);
            distance = Mathf.Clamp01(distance / maxInmateDistance);
            float distanceFactor = 1 - distance;

            Vector2 position = new Vector2(boid.transform.position.x, boid.transform.position.z);

            Vector2 direction = new Vector2(boid.transform.forward.x, boid.transform.forward.z);
            cumulativeDirection += direction * distanceFactor;
        }

        Vector2 avgDirection = cumulativeDirection / Inmates.Count;

        averageDirection.Invoke(avgDirection.normalized);
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
