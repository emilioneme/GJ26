using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PulsingLight : MonoBehaviour
{
    [Header("InstensityStrenght")]
    [SerializeField] float currentStrenght = 0;

    [Header("Pulse")]
    [SerializeField] float pulseDuration = .5f;
    [SerializeField] float pulseCoodlwon = 1f;
    [SerializeField] float pulseSpeed = 10f;

    [Header("AlertLevel")]
    [SerializeField] float maxAlert = 2;
    [SerializeField] float maxStrength = 1;

    [Header("Debug +")]
    [SerializeField] float desiredStrenght = 1;
    [SerializeField] Light pulseLight;
    [SerializeField] GameObject alarmSoundPrefab;
    [SerializeField] GameObject alarmSoundInstance;


    public float lasTimePulsed = 0;

    private void Update()
    {
        desiredStrenght = UserData.Instance.alertLevel / maxAlert;

        if (Time.time - lasTimePulsed > pulseCoodlwon) 
        {
            lasTimePulsed = Time.time;
            if(currentStrenght != 0) 
            {
                StartCoroutine(pulseRoutine());
            }
                
        }

        pulseLight.intensity = currentStrenght * maxStrength;

    }

    IEnumerator pulseRoutine()
    {
        currentStrenght = 0;
        alarmSoundInstance = Instantiate(alarmSoundPrefab);
        AudioSource audio = alarmSoundPrefab.GetComponent<AudioSource>();

        while (currentStrenght < desiredStrenght)
        {
            currentStrenght += Time.deltaTime * pulseSpeed;
            audio.volume = currentStrenght * desiredStrenght;
            yield return null;
        
        }
        yield return new WaitForSeconds(pulseDuration);

        while (currentStrenght > 0)
        {
            currentStrenght -= Time.deltaTime * pulseSpeed;
            audio.volume = currentStrenght * desiredStrenght;
            yield return null;

        }
        Destroy(alarmSoundInstance);
    }

}
