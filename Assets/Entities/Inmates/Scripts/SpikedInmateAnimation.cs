using UnityEngine;

public class SpikedInmateAnimation : MonoBehaviour
{
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetFloat("Offset", Random.Range(0f, 1f));
    }

}
