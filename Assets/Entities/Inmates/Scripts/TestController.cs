using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TestController : MonoBehaviour
{
    public CharacterController cc;
    public InmateInput input;
    
    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }
    // Update is called once per frame
    void Update()
    {
        cc.Move(input.forwardDirection * Time.deltaTime);
        // cc.transform.rotation = Quaternion.LookRotation(input.forwardDirection);
    }
}
