using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DirectionCone : MonoBehaviour
{
    GameObject self;
    public UnityEvent<Vector2> averageDirection;

    [Header("Cone Settings")]
    public float radius = 10f;
    [Range(0, 180)]
    public float angle = 45f;
    public LayerMask inmateLayer;

    [SerializeField] List<GameObject> inmates = new List<GameObject>();
    Collider[] results = new Collider[50];

    void Awake()
    {
        if (self == null)
            self = transform.root.gameObject;
    }

    void Update()
    {
        inmates.Clear();

        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            radius,
            results,
            inmateLayer,
            QueryTriggerInteraction.Ignore
        );

        Vector3 forward = transform.forward;
        float cosAngle = Mathf.Cos(angle * Mathf.Deg2Rad);

        for (int i = 0; i < count; i++)
        {
            GameObject go = results[i].gameObject;
            if (go == self) continue;

            Vector3 dir = (go.transform.position - transform.position).normalized;

            float dot = Vector3.Dot(forward, dir);

            if (dot > cosAngle)
                inmates.Add(go);
        }

        // --- your original averaging logic ---
        if (inmates.Count == 0)
        {
            averageDirection.Invoke(Vector2.zero);
            return;
        }

        Vector2 cumulative = Vector2.zero;

        foreach (GameObject boid in inmates)
        {
            Vector2 d = new Vector2(boid.transform.forward.x, boid.transform.forward.z);
            cumulative += d;
        }

        Vector2 avg = (cumulative / inmates.Count).normalized;
        averageDirection.Invoke(avg);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);

        Vector3 left = Quaternion.Euler(0, -angle, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, angle, 0) * transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, left * radius);
        Gizmos.DrawRay(transform.position, right * radius);
    }

}
