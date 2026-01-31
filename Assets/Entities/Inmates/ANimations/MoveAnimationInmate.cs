using System;
using UnityEngine;

public class MoveAnimationInmate : MonoBehaviour
{
    public CharacterController cc;
    private void OnAnimatorMove()
    {
        Vector3 velocity = GetComponent<Animator>().deltaPosition;
        cc.Move(velocity);
    }
}
