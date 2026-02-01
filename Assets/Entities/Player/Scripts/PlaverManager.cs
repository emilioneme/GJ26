using DG.Tweening;
using System.Collections;
using TMPro;
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
    [SerializeField] Transform TowerLight;
    [SerializeField] GameObject brdigeBlocker;
    [SerializeField] float maxEndingDist;
    [SerializeField] Color endingColor = Color.white;

    [Header("Layers")]
    [SerializeField] float interactionDistance = 2f;
    [SerializeField] float interactionRadius = 0.5f;

    [Header("Layers")]
    

    [SerializeField] LayerMask bridgeLayer;
    [SerializeField] LayerMask helper2Layer;
    [SerializeField] LayerMask valuableLayer;
    [SerializeField] LayerMask rioterLayer;

    [Header("Dialogue")]
    [SerializeField] LayerMask guardLayer;
    [SerializeField][TextArea] string[] guardDialogues;
    [SerializeField] LayerMask npcLayer;
    [SerializeField][TextArea] string[] npcDialogues;

    [Header("Level1")]
    [SerializeField] LayerMask alarmLayer;
    [SerializeField][TextArea] string[] alarmTriggeredInnerDialogues;
    [SerializeField][TextArea] string[] alarmInnerDialogues;

    [SerializeField] LayerMask helper1Layer;
    [SerializeField][TextArea] string[] npcLadderHintDialogues;
    [SerializeField][TextArea] string[] npcLadderParanoidDialogues;

    [SerializeField] LayerMask ladderLayer;
    [SerializeField][TextArea] string[] ladderLockedInnerDialogue;
    [SerializeField][TextArea] string[] ladderUnlockedInnerDialogue;

    [Header("Level2")]
    [SerializeField][TextArea] string[] rioterHintNPCDialogues; // tells u about this guy planning a riot and bridge

    [SerializeField][TextArea] string[] valueableFoundDialogue; //like a prison light manifesto manifesto
    [SerializeField][TextArea] string[] valueableCollecterDialogue;

    [SerializeField][TextArea] string[] rioterManifestoDialogue;

    [SerializeField][TextArea] string[] bridgeLockedDialogues;


    [Header("text effect")]
    [SerializeField] float textCharacterCooldown = .1f;
    [SerializeField] float textCharacterRemoveCooldown = .1f;
    [SerializeField] float textCooldownFactor = .3f;

    bool ladderUnlocked = false;

    bool bridgeLocked = true;
    bool hasValuable = false;

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
                    SetDialogue(npcLadderHintDialogues);
                else
                    SetDialogue(npcLadderParanoidDialogues);
            }

            // Alarm
            if (((1 << layer) & alarmLayer) != 0) 
            {
                if (!ladderUnlocked) 
                {
                    SetDialogue(alarmInnerDialogues);
                    if (playerInputHandler.InteractAction.WasCompletedThisFrame()) 
                    {
                        UnlockLadder();
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

            //Level 2 /////////////////////////
            //Helper2
            if (((1 << layer) & helper2Layer) != 0)
            {
                if (bridgeLocked)
                    SetDialogue(rioterHintNPCDialogues);
                
            }

            //Valuable
            if (((1 << layer) & valuableLayer) != 0)
            {
                SetDialogue(valueableFoundDialogue);

                if(playerInputHandler.InteractAction.WasCompletedThisFrame()) 
                {
                    CollectValuable(hit);
                }
            }

            //Rioter
            if (((1 << layer) & rioterLayer) != 0)
            {
                if (hasValuable && bridgeLocked) 
                {
                    if (playerInputHandler.InteractAction.WasCompletedThisFrame()) 
                    {
                        GiveValuable();
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
            float distance = Mathf.Clamp(Vector3.Distance(transform.position, TowerLight.position), 0, maxEndingDist) / maxEndingDist;
            float inverse = 1 - distance;
            FadeImage.color = Color.Lerp(Color.clear, endingColor, inverse);
        }

    }

    public void UnlockBridge() 
    {
        hasValuable = true;
    }

    public void UnlockLadder()
    {
        ForceDialogue(alarmTriggeredInnerDialogues);
        ladderUnlocked = true;
    }

    public void GiveValuable() 
    {
        bridgeLocked = true;
        brdigeBlocker.SetActive(false);
        FadeImage.color = Color.clear;
        ForceDialogue(rioterManifestoDialogue);
        riotStarted.Invoke(transform.position);
    }

    public void CollectValuable(RaycastHit hit) 
    {
        Destroy(hit.transform);
        ForceDialogue(valueableCollecterDialogue);
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

        SetDialogue(dialogues);
    }

    void SetDialogue(string[] dialogues)
    {
        int i = Random.Range(0, dialogues.Length);
        string text = dialogues[0];
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

