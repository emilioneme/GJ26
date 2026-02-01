using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlaverManager : MonoBehaviour
{
    [SerializeField] Transform pivot;
    [SerializeField] GameObject FadeImageGO;
    [SerializeField] Image FadeImage;
    //[SerializeField] GameObject InteractionTextGO;
    [SerializeField] TMP_Text InteractionText;

    [SerializeField] float interactionDistance = 2f;
    [SerializeField] float interactionRadius = 0.5f;

    [SerializeField] LayerMask ladderLayer;
    [SerializeField] LayerMask alarmLayer;
    [SerializeField] LayerMask NPCLayer;
    [SerializeField] LayerMask guardLayer;

    [SerializeField][TextArea] string lockedLadderText = "'Too Risky'";
    [SerializeField][TextArea] string ladderText = "'might as well while I can'";
    [SerializeField][TextArea] string alarmTextText = "'fuck maybe a should trigger this alarm...";
    [SerializeField][TextArea] string prisonGuardText = "Guard: 'keep walking'";
    [SerializeField][TextArea] string NPC = "Alfred: 'you know what they do to people like us if you dont blend in'";

    bool ladderUnlocked = false;

    float textDuration = 1f;
    Coroutine textRoutine;

    private void Start()
    {
        FadeImageGO.SetActive(true);
        FadeImage.DOColor(Color.clear, 1f)
        .OnComplete(() =>
        {
            FadeImageGO.SetActive(false);
        });
    }


    public void FixedUpdate()
    {
        RaycastHit hit;

        if (Physics.SphereCast(pivot.transform.position, interactionRadius, pivot.transform.forward, out hit, interactionDistance))
        {
            int layer = hit.collider.gameObject.layer; //string layerName = LayerMask.LayerToName(layer);
            //((1 << layer) is a bit flag

            if (((1 << layer) & alarmLayer) != 0) 
            {
                if (!ladderUnlocked) 
                {
                    SetText(lockedLadderText);
                }
            }

            if (((1 << layer) & ladderLayer) != 0) 
            {
                if (ladderUnlocked) 
                {
                    SetText(lockedLadderText);
                }
                else 
                {
                    SetText(ladderText);
                }
            }

            if (((1 << layer) & guardLayer) != 0)
            {
                SetText(prisonGuardText);
            }

            if (((1 << layer) & NPCLayer) != 0)
            {
                SetText(NPC);
            }

            return;
        }
    }

    void SetText(string text) 
    {
        InteractionText.text = text;
        if(textRoutine != null) 
        {
            StopCoroutine(textRoutine);
        }
        textRoutine = StartCoroutine(TextCouroutine(text));
    }

    IEnumerator TextCouroutine(string text) 
    {
        float cooldown = 0;
        //string currentText = text;
        while (cooldown < textDuration) 
        {
            cooldown += Time.deltaTime;
            yield return null;
        }
        InteractionText.text = "";
        textRoutine = null;
    }

    public void ClimbLadder() 
    {
        FadeImageGO.SetActive(true);
        FadeImage.color = Color.clear;
        FadeImage.DOColor(Color.black, 1f)
        .OnComplete(() =>
        {
            LoadScene("Level2");
        });
    }

    public void LoadScene(string sceneToLoad) 
    {
        SceneManager.LoadScene(sceneToLoad);
    }
    public void UnlockLadder()
    {
        ladderUnlocked = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pivot.transform.position + pivot.transform.forward * interactionDistance, interactionRadius);
    }
}

