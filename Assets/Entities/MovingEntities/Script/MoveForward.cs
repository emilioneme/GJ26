using UnityEngine;
using UnityEngine.Events;

public class MoveForward : MonoBehaviour
{
    [SerializeField] UnityEvent<Vector2> onMove;

    private void Update()
    {
        onMove.Invoke(new Vector2(0, 1));
    }

}
