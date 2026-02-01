using UnityEngine;

public class SpotlightCollider : MonoBehaviour
{
    [SerializeField] float damageCooldown = 0.5f;
    [SerializeField] float minBlendingEfficacy = 0.6f;
    [SerializeField] float healMultiplier = 0.3f;
    [SerializeField] float damageMultiplier = 3f;

    BlendingHandler blendingHandler;

    public bool playerTarget;
    
    
    float lastTimeDamaged = 0;

    public void Update()
    {
        if (playerTarget)
        {
            blendingHandler = GameManager.Instance.player.GetComponent<BlendingHandler>();
        }

        if (blendingHandler == null)
        {
            blendingHandler = GameManager.Instance.player.GetComponent<BlendingHandler>();
            //inverted damage / logic to lower alert if blending in
            if (Time.time - lastTimeDamaged > damageCooldown)
            {
                //Debug.Log("BlendingEfficacy: " + blendingHandler.currentBlendingEfficacy);
                if(blendingHandler.currentBlendingEfficacy > minBlendingEfficacy)
                {
                    Debug.Log("blending in to heal");
                    float heal = Mathf.InverseLerp(
                        minBlendingEfficacy,
                        1f,
                        blendingHandler.currentBlendingEfficacy
                    ) * healMultiplier;
                    Debug.Log("heal: " + -heal);
                    GameManager.Instance.RaiseAlert(-heal);
                    //Debug.Log(damage);
                    lastTimeDamaged = Time.time;
                }
            }
            blendingHandler = null;
            return;
        }
            

        if (Time.time - lastTimeDamaged > damageCooldown)
        {
            //Debug.Log("BlendingEfficacy: " + blendingHandler.currentBlendingEfficacy);
            if(blendingHandler.currentBlendingEfficacy < minBlendingEfficacy)
            {
                float damage = Mathf.InverseLerp(1 - minBlendingEfficacy, -1, blendingHandler.currentBlendingEfficacy) * damageMultiplier;
                GameManager.Instance.RaiseAlert(damage);
                Debug.Log(damage);
                lastTimeDamaged = Time.time;
            }
        }

    }

    public void setPlayerTarget(bool setPlayerTarget)
    {
        if (setPlayerTarget)
        {
            blendingHandler = GameManager.Instance.player.GetComponent<BlendingHandler>();
        } else
        {
            blendingHandler = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BlendingHandler bh))
        {
            blendingHandler = bh;
            Debug.Log("Entered spotlight");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out BlendingHandler bh) && bh == blendingHandler)
        {
            blendingHandler = null;
            Debug.Log("Exited spotlight");
        }
    }
}
