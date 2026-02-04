using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BlendingHandler : MonoBehaviour
{
    [Header("blendingPArameter")]
    public float currentBlendingEfficacy = 0;
    public float previousBlendingEfficacy = 0;
    public float normalizedBlend = 1;

    [Header("DangerZones")]
    [SerializeField][Range(-1, -.5f)] float minBlendEfficacyForAlert = -.93f;
    [SerializeField][Range(0, 100)] float alertRaiseAmount = 3f;
    [SerializeField] float alertCooldown = 3f;
    [SerializeField] float lastAlert = 15;

    [Header("smooth blenmding")]
    public float blendingSpeed = 1;
    public float minSpeed = 1;
    public float maxSpeed = 1;

    [Header("punsihments and rewards")]
    [SerializeField][Range(-1, 0)] float sprintingPunishment = -.3f;
    [SerializeField][Range(-1, 0)] float notMatchingWalkPunshismet = -.5f;
    [SerializeField][Range(0, 1)] float maxInmateCountReward = .5f;
    [SerializeField][Range(0, 1)] float noWacthersReward = .5f;
    [SerializeField][Range(-1, 0)] float watchersPunishmentFactor = -.5f;

    [SerializeField] int maxInmatesForCount = 20;
    [SerializeField] AnimationCurve inmatedCountRwardCurve;
    float totalPunishment = 0;

    bool watched;

    [Header("Being Watched Ranges")]

    [Header("info")]
    [SerializeField] MovingController controller;
    [SerializeField] List<WatchingEntitiy> watchers;
    [Header("stanting group")]
    public GameObject standingTarget = null;

    [Header("Sound")]
    [SerializeField]float startingPitch = 1;
    [SerializeField] AudioSource blendingSound;
    [SerializeField] GameObject watchedSound;
    [SerializeField] float watchedDisableCooldown;

    private void Update()
    {
        SmoothenBlending();
        normalizedBlend = (currentBlendingEfficacy + 1) / 2;
        blendingSound.pitch = startingPitch + Mathf.InverseLerp(1, 0, normalizedBlend);

        if(currentBlendingEfficacy < minBlendEfficacyForAlert && Time.time - lastAlert < alertCooldown) 
        {
            AudioManager.Instance.RaiseAlertSound();
            GameManager.Instance.RaiseAlert(20);

        }
    }

    public void UpdateBlendEfficacy(Vector2 avgDirection, bool walking, int inmateCount) 
    {
        totalPunishment = 0;

        SprintingPunishment();
        InmateCountReward(inmateCount);

        //in standing group
        if (standingTarget != null)
        {
            StandingGroup();
        } 
        else //not in standing group
        {
            DirectionEfficacy(avgDirection);

            WalkingPunishment(walking);
        }

        WactherSystem();

        previousBlendingEfficacy = Mathf.Clamp(previousBlendingEfficacy + totalPunishment, -1, 1);
    }


    public void DirectionEfficacy(Vector2 avgDirection) 
    {
        if (avgDirection.sqrMagnitude < 0.0001f)
        {
            previousBlendingEfficacy = -1;
            return;
        }
        Vector2 currentDirection = new Vector2(transform.forward.x, transform.forward.z);
        float dot = Vector2.Dot(currentDirection.normalized, avgDirection.normalized);
        previousBlendingEfficacy = dot;
    }

    void SprintingPunishment() 
    {
        if (controller.sprintCoroutine != null)
            totalPunishment += sprintingPunishment;
    }

    void WalkingPunishment(bool walking)
    {
        if ((controller.moveInput.magnitude > 0.4f) != walking)
            totalPunishment += notMatchingWalkPunshismet;
    }

    void InmateCountReward(int inmateCount) 
    {
        float nomralizedCount = Mathf.Clamp01(inmateCount / maxInmateCountReward);
        float curvedReward = inmatedCountRwardCurve.Evaluate(nomralizedCount);
        totalPunishment += maxInmateCountReward;
    }

    void WactherSystem() 
    {
        if (MaxWatchingStrenght() == 0) 
        {
            totalPunishment += noWacthersReward;
            watchedSound.SetActive(false);
        }
        else 
        {
            watchedSound.SetActive(true);
            totalPunishment += MaxWatchingStrenght() * watchersPunishmentFactor;
        }

    }

    float MaxWatchingStrenght()
    {
        float max = 0;

        if (watchers.Count <= 0)
        {
            watched = false;
            return max;
        }

        watched = true;
        foreach (WatchingEntitiy we in watchers)
        {
            max = Mathf.Max(max, we.wathcingStrength);
        }
        return max;
    }


    #region STandng Group
    void StandingGroup() 
    {
        Vector2 forward2D = new Vector2(transform.forward.x, transform.forward.z).normalized;

        Vector2 toTarget2D = new Vector2(
            standingTarget.transform.position.x - transform.position.x,
            standingTarget.transform.position.z - transform.position.z
        ).normalized;

        float dot = Vector2.Dot(forward2D, toTarget2D);
        previousBlendingEfficacy = dot;
    }
    #endregion

    #region Smooth
    public void SmoothenBlending()
    {
        float difference = Mathf.Abs(previousBlendingEfficacy - currentBlendingEfficacy);

        float speed = Mathf.Max(minSpeed, difference * blendingSpeed);
        //speed = Mathf.Min(maxSpeed, difference * blendingSpeed);

        currentBlendingEfficacy = Mathf.MoveTowards(
            currentBlendingEfficacy,
            previousBlendingEfficacy,
            speed * Time.deltaTime
        );
    }
    #endregion

    #region WactherCount

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out WatchingEntitiy we)) 
        {
            if (!watchers.Contains(we))
                watchers.Add(we);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out WatchingEntitiy we))
        {
            if (watchers.Contains(we))
                watchers.Remove(we);
        }
    }
    #endregion

}
