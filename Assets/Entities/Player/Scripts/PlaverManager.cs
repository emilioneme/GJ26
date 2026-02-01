using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class PlaverManager : MonoBehaviour
{
    [SerializeField] Transform pivot;
    [SerializeField] GameObject FadeImageGO;
    [SerializeField] UnityEngine.UI.Image FadeImage;
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
    [SerializeField][TextArea] string alarmTiggeredTextText = "'fuck maybe a should trigger this alarm...";
    [SerializeField][TextArea] string prisonGuardText = "Guard: 'keep walking'";
    [SerializeField][TextArea] string NPCHint = "Alfred: 'you know what they do to people like us if you dont blend in'";
    [SerializeField][TextArea] string NPCAfterHint = "Alfred: 'you know what they do to people like us if you dont blend in'";

    [SerializeField] float textCharacterCooldown = .1f;
    [SerializeField] float textCooldownFactor = .3f;

    bool ladderUnlocked = false;

    Coroutine textAppearRoutine;
    Coroutine textDisappearRoutine;

    PlayerInputHandler playerInputHandler;

    private void Awake()
    {
        playerInputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Start()
    {
        FadeImageGO.SetActive(true);
        FadeImage.DOColor(Color.clear, 1f)
        .OnComplete(() =>
        {
            FadeImageGO.SetActive(false);
        });
    }


    public void Update()
    {
        RaycastHit hit;
        if (Physics.SphereCast(pivot.transform.position, interactionRadius, pivot.transform.forward, out hit, interactionDistance))
        {
            int layer = hit.collider.gameObject.layer; //string layerName = LayerMask.LayerToName(layer);
            //((1 << layer) is a bit flag

            // Alarm
            if (((1 << layer) & alarmLayer) != 0) 
            {
                if (!ladderUnlocked) 
                {
                    SetText(alarmTextText);
                    if (playerInputHandler.InteractAction.WasCompletedThisFrame()) 
                    {
                        UnlockLadder();
                    }
                }
                else 
                {
                    SetText(alarmTiggeredTextText);
                }
            }

            //Ladder
            if (((1 << layer) & ladderLayer) != 0) 
            {
                if (!ladderUnlocked) 
                {
                    SetText(lockedLadderText);
                }
                else 
                {
                    SetText(ladderText);
                    if (playerInputHandler.InteractAction.WasCompletedThisFrame())
                    {
                        ClimbLadder();
                    }
                }
            }

            //Guard
            if (((1 << layer) & guardLayer) != 0)
            {
                SetText(prisonGuardText);
            }

            //NPC
            if (((1 << layer) & NPCLayer) != 0)
            {
                if(!ladderUnlocked)
                    SetText(NPCHint);
                else
                    SetText(NPCAfterHint);
            }

            return;
        }
    }

    #region Text Management
    void SetText(string text)
    {
        if (textAppearRoutine == null) 
        {
            textAppearRoutine = StartCoroutine(TextAppearCoroutine(text));

            if (textDisappearRoutine != null) 
            {
                StopCoroutine(textDisappearRoutine);
            }
        }
    }

    IEnumerator TextAppearCoroutine(string text)
    {
        InteractionText.text = string.Empty;

        for (int i = 0; i < text.Length; i++)
        {
            InteractionText.text += text[i];
            yield return new WaitForSeconds(textCharacterCooldown);
        }

        float textCooldown = text.Length * textCooldownFactor;
        yield return new WaitForSeconds(textCooldown);

        textAppearRoutine = null;
        textDisappearRoutine = StartCoroutine(TextDissapearCoroutine(text));
    }

    IEnumerator TextDissapearCoroutine(string text)
    {
        string currentText = InteractionText.text;

        while (currentText.Length > 0)
        {
            currentText = currentText.Remove(currentText.Length - 1);
            InteractionText.text = currentText;
            yield return new WaitForSeconds(textCharacterCooldown);
        }

        textDisappearRoutine = null;
    }


    #endregion

    public void UnlockLadder()
    {
        ladderUnlocked = true;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pivot.transform.position + pivot.transform.forward * interactionDistance, interactionRadius);
    }
}

