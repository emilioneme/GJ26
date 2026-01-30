using UnityEngine;

namespace Entities.Inmates
{
    public class InmateInput : MonoBehaviour
    {
        [SerializeField] public Vector3 forwardDirection;

        void Awake()
        {
            Vector3 forwardStart = Random.insideUnitSphere.normalized;
            forwardDirection = forwardStart - Vector3.Dot(forwardStart, Vector3.up)*Vector3.up;
        }

        public void UpdateMovementDirection()
        {
            forwardDirection = Random.insideUnitCircle.normalized;
        }
    
    }
}