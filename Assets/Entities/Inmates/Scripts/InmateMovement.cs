using System;
using UnityEngine;

public class InmateMovement : MonoBehaviour
{
    CharacterController cc;

    [SerializeField] float speed = 1;

    [SerializeField] Vector2 desiredDirection;

    void Awake()
    {
       cc = gameObject.GetComponent<CharacterController>(); 
    }

    void Update()
    {
        Vector3 moveDirection3 = new Vector3(desiredDirection.x, 0, desiredDirection.y).normalized;
        cc.Move(moveDirection3 * speed * Time.deltaTime);
    }

    public void UpdateMoveDirection(Vector2 direction)
    {
        desiredDirection = direction;
    }
}
