using DG.Tweening;
using System.Collections;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
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

    public UnityEvent<Vector3> alarmTriggered;
    public UnityEvent<Vector3> riotStarted;
    public UnityEvent climbingLadder; // for sounds

    [Header("Tower Ending")]
    [SerializeField] float maxEndingDist;
    [SerializeField] Color endingColor = Color.white;

    [Header("interactiveness")]
    [SerializeField] float interactionDistance = 2f;
    [SerializeField] float interactionRadius = 0.5f;


    [Category("Leveless Dialogue")]
    [SerializeField] LayerMask guardLayer;
    [SerializeField][TextArea] string[] guardDialogues;
    [SerializeField] LayerMask npcLayer;
    [SerializeField][TextArea] string[] npcDialogues;

    [Category("Level1")]
    [SerializeField] LayerMask alarmLayer;
    [SerializeField][TextArea] string[] alarmTriggeredInnerDialogues;
    [SerializeField][TextArea] string[] alarmInnerDialogues;

    [SerializeField] LayerMask helper1Layer;
    [SerializeField][TextArea] string[] Helper1HintDialogues;
    [SerializeField][TextArea] string[] Helper1AlertDialogues;

    [SerializeField] LayerMask ladderLayer;
    [SerializeField][TextArea] string[] ladderLockedInnerDialogue;
    [SerializeField][TextArea] string[] ladderUnlockedInnerDialogue;

    [Category("Level2")]
    [SerializeField] LayerMask helper2Layer;
    [SerializeField][TextArea] string[] helper2ValuableDialogue; // tells u about this guy planning a riot and bridge
    [SerializeField][TextArea] string[] helper2RioterDialogue; // tells u about this guy planning a riot and bridge

    [SerializeField] LayerMask valuableLayer;
    [SerializeField][TextArea] string[] valuableDialogue; //like a prison light manifesto manifesto
    [SerializeField][TextArea] string[] valuableCollectedDialogue;

    [SerializeField] LayerMask rioterLayer;
    [SerializeField][TextArea] string[] rioterDialogue;
    [SerializeField][TextArea] string[] rioterChantDialogue;

    [SerializeField] LayerMask bridgeLayer;
    [SerializeField][TextArea] string[] bridgeLockedDialogues;


    [Category("text effect")]
    [SerializeField] float textCharacterCooldown = .1f;
    [SerializeField] float textCharacterRemoveCooldown = .1f;
    [SerializeField] float textCooldownFactor = .3f;

    bool ladderUnlocked = false;

    bool bridgeLocked = true;
    bool hasValuable = false;

    string lastDialogue;

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
            int layer = hit.collider.gameObject.layer; 
            string layerName = LayerMask.LayerToName(layer);
            Debug.Log(layerName);
            //((1 << layer) is a bit flag

            //Guards
            if (((1 << layer) & guardLayer) != 0)
            {
                SetDialogue(guardDialogues);
            }

            //Guards
            if (((1 << layer) & npcLayer) != 0)
            {
                SetDialogue(npcDialogues);
            }

            //Level 1 /////////////////////////

            //Helper 1
            if (((1 << layer) & helper1Layer) != 0)
            {
                if (!ladderUnlocked)
                    SetDialogue(Helper1HintDialogues);
                else
                    SetDialogue(Helper1AlertDialogues);
            }

            // Alarm
            if (((1 << layer) & alarmLayer) != 0) 
            {
                if (!ladderUnlocked) 
                {
                    SetDialogue(alarmInnerDialogues);
                    Debug.Log("alarm inner dialogue");
                    if (playerInputHandler.InteractAction.WasCompletedThisFrame()) 
                    {
                        ladderUnlocked = true;
                        ForceDialogue(alarmTriggeredInnerDialogues);
                        alarmTriggered.Invoke(transform.position);
                        
                    }
                }
            }

            //Ladder
            if (((1 << layer) & ladderLayer) != 0) 
            {
                if (!ladderUnlocked) 
                {
                    SetDialogue(ladderLockedInnerDialogue);
                }
                else 
                {
                    SetDialogue(ladderUnlockedInnerDialogue);
                    if (playerInputHandler.InteractAction.WasCompletedThisFrame())
                    {
                        ClimbLadder();
                    }
                }
            }

            //Level 2 /////////////////////////////////////////////////////////////


            //Helper2
            if (((1 << layer) & helper2Layer) != 0)
            {
                if (bridgeLocked && !hasValuable)
                    SetDialogue(helper2ValuableDialogue);
                else if (bridgeLocked)
                    SetDialogue(helper2ValuableDialogue);
            }

            //Valuable
            if (((1 << layer) & valuableLayer) != 0)
            {
                SetDialogue(valuableDialogue);

                if(playerInputHandler.InteractAction.WasCompletedThisFrame()) 
                {
                    GameObject go = hit.transform.gameObject;
                    Destroy(go, 1f);
                    ForceDialogue(valuableCollectedDialogue);
                }
            }

            //Rioter
            if (((1 << layer) & rioterLayer) != 0)
            {
                if (hasValuable && bridgeLocked) 
                {
                    SetDialogue(rioterDialogue);

                    if (playerInputHandler.InteractAction.WasCompletedThisFrame()) 
                    {
                        bridgeLocked = true;
                        GameManager.Instance.BridgeBlocker.SetActive(false);
                        FadeImage.color = Color.clear;
                        riotStarted.Invoke(transform.position);
                        ForceDialogue(rioterChantDialogue);
                    }
                }
            }

            //Bridge
            if (((1 << layer) & bridgeLayer) != 0) 
            {
                if (bridgeLocked) 
                {
                    SetDialogue(bridgeLockedDialogues);
                }
            }
        }

        if (!bridgeLocked) 
        {
            float distance = Mathf.Clamp(Vector3.Distance(transform.position, GameManager.Instance.EndingTransform.position), 0, maxEndingDist) / maxEndingDist;
            float inverse = 1 - distance;
            FadeImage.color = Color.Lerp(Color.clear, endingColor, inverse);
        }

    }

    public void UnlockBridge() 
    {
        hasValuable = true;
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

    #region Text Management
    void ForceDialogue(string[] dialogues)
    {
        if (textDisappearRoutine != null)
            StopCoroutine(textDisappearRoutine);
        if (textAppearRoutine != null)
            StopCoroutine(textAppearRoutine);

        InteractionText.text = "";

        lastDialogue = null; // or "" � just something that won't match
        SetDialogue(dialogues);
    }

    void SetDialogue(string[] dialogues)
    {
        int i = Random.Range(0, dialogues.Length);
        string text = dialogues[i];

        if (lastDialogue == text)
            return;
        lastDialogue = text;

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
            yield return new WaitForSeconds(textCharacterRemoveCooldown);
        }

        textDisappearRoutine = null;
    }


    #endregion

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

