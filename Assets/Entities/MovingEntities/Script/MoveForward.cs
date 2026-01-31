using UnityEngine;
using UnityEngine.Events;

public class MoveForward : MonoBehaviour
{
    [SerializeField] UnityEvent<Vector2> onMove;
    [SerializeField] UnityEvent changedSpeed;

    [SerializeField] float idlePropability = 0.3f;

    bool isMoving = true;


    private void Update()
    {
        onMove.Invoke(isMoving ? new Vector2(0, 1) : Vector2.zero);
    }

    public void ChangeBehaviour()
    {
        float RandomValue = Random.Range(0f, 1f);
        bool prevIsMoving = isMoving;
        isMoving = RandomValue > idlePropability;
        if(prevIsMoving != isMoving)
            changedSpeed.Invoke();
    }

}
