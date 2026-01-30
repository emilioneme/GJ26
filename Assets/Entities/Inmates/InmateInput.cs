using UnityEngine;

namespace Entities.Inmates
{
    public class InmateInput : MonoBehaviour
    {
        [SerializeField] public Vector3 forwardDirection;

        public void UpdateMovementDirection()
        {
            forwardDirection = Random.insideUnitCircle.normalized;
        }
    
    }
}