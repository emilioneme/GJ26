using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovingController : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] float maxSprintSpeed = 5f;
    float sprintSpeed = 0;

    [SerializeField] float sprintIncreaseSpeed = 1;
    [SerializeField] float gravity = -9.81f;

    public Vector3 moveInput;

    [SerializeField] bool canMove = true;

    CharacterController cc;
    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if(cc.isGrounded == false)
            moveInput.y += gravity * Time.deltaTime;
        else
            moveInput.y = 0f;

        if(canMove)
            cc.Move(transform.rotation * moveInput * Time.deltaTime);
    }

    public void UpdateMoveEntity(Vector2 direction)
    {
        Vector2 input = direction.normalized * (moveSpeed + sprintSpeed);
        moveInput.x = input.x;
        moveInput.z = input.y;
    }

    
    public Coroutine sprintCoroutine;
    public void UpdateSprint() 
    {
        if(sprintCoroutine != null) 
        {
            StopCoroutine(sprintCoroutine);
            sprintCoroutine = null;
        }

        sprintCoroutine = StartCoroutine(SprintIncrease());
    }

    IEnumerator SprintIncrease() 
    {
        while (sprintSpeed < maxSprintSpeed)
        {
            sprintSpeed = Mathf.Clamp(sprintSpeed + sprintIncreaseSpeed * Time.deltaTime, 0f, maxSprintSpeed);
            yield return null;
        }

        while (sprintSpeed > 0)
        {
            sprintSpeed = Mathf.Clamp(sprintSpeed - sprintIncreaseSpeed * Time.deltaTime, 0f, maxSprintSpeed);
            yield return null;
        }

        sprintSpeed = 0;
        sprintCoroutine = null;
    }
    
}
