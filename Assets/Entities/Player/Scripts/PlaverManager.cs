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
    [SerializeField] UnityEngine.UI.Image VignneteImage;
    //[SerializeField] GameObject InteractionTextGO;
    [SerializeField] TMP_Text InteractionText;

    public UnityEvent<Vector3> alarmTriggered;
    public UnityEvent<Vector3> riotStarted;
    public UnityEvent climbingLadder; // for sounds
    public UnityEvent gotWeapon;
    public UnityEvent reachedEnding;

    [SerializeField] BlendingHandler BlendingHandler;

    [Header("VIgnette")]
    [SerializeField] Color fromColor;
    [SerializeField] Color toColor;
    [SerializeField] AnimationCurve blendingCurve;


    [Header("Tower Ending")]
    [SerializeField] float maxEndingDist;
    [SerializeField] Color endingColor = Color.white;
    [SerializeField] float alphaFactor = .5f;

    [Header("interactiveness")]
    [SerializeField] float interactionDistance = 2f;
    [SerializeField] float interactionRadius = 0.5f;


    [Category("Leveless Dialogue")]
    [SerializeField][TextArea] string[] introDialogues;
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
    [SerializeField][TextArea] string[] hasValuableDialogue; //like a prison light manifesto manifesto
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

    bool hasAccessToBrdige = false;
    bool hasValuable = false;

    string[] lastDialogues;

    Coroutine textAppearRoutine;
    Coroutine textDisappearRoutine;

    PlayerInputHandler playerInputHandler;
    [SerializeField] TMP_Text DebugText;

    [SerializeField] bool showDeveloper = false;
    bool developerMode = false;

    private void Awake()
    {
        playerInputHandler = GetComponent<PlayerInputHandler>();
#if UNITY_EDITOR
        developerMode = true;
#endif
    }

    private void Start()
    {
        if (developerMode && showDeveloper) DebugText.enabled = true;
        else DebugText.enabled = false;

        FadeImageGO.SetActive(true);
        FadeImage.color = Color.black;
        FadeImage.DOColor(Color.clear, 1f)
        .OnComplete(() =>
        {
            FadeImageGO.SetActive(false);
        });
        ForceDialogue(introDialogues);

        VignneteImage.color = fromColor;
    }


    public void Update()
    {
        DebugText.text = "Blednign Efficacy: " + BlendingHandler.currentBlendingEfficacy + "\n"
            + "\n" + "alignment: " + BlendingHandler.alignmentEfficacy
            + "\n" + "sprinting: " + BlendingHandler.currentSprintPunhisment
            //+ "\n" + "macth walking: " + BlendingHandler.currentMatchingWalkPunishment
            + "\n" + "inmate count: " + BlendingHandler.currentInmateCuuntReward
            + "\n" + "wacther: " + BlendingHandler.currentWactherPunhisment
            + "\n" + "\n" + "Alert Level: " + UserData.Instance.alertLevel
            + "\n" + "Alert Bar: " + UserData.Instance.alertBarAmount;

        float blendingEfficacy = Mathf.InverseLerp(1, -1, BlendingHandler.currentBlendingEfficacy);
        float t = blendingCurve.Evaluate(blendingEfficacy);
        VignneteImage.color = Color.Lerp(fromColor, toColor, t);
        //float a = t * alphaFactor;
        //VignneteImage.color = new Color(VignneteImage.color.r, VignneteImage.color.g, VignneteImage.color.b, t);


        RaycastHit hit;
        if (Physics.SphereCast(pivot.transform.position, interactionRadius, pivot.transform.forward, out hit, interactionDistance))
        {
            int layer = hit.collider.gameObject.layer; 
            string layerName = LayerMask.LayerToName(layer);
            //((1 << layer) is a bit flag

            //Guards
            if (((1 << layer) & guardLayer) != 0)
            {
                ForceDialogue(guardDialogues);
            }

            //Guards
            if (((1 << layer) & npcLayer) != 0)
            {
                ForceDialogue(npcDialogues);
            }

            //Level 1 /////////////////////////

            //Helper 1
            if (((1 << layer) & helper1Layer) != 0)
            {
                if (!ladderUnlocked)
                    ForceDialogue(Helper1HintDialogues);
                else
                    ForceDialogue(Helper1AlertDialogues);
            }

            // Alarm
            if (((1 << layer) & alarmLayer) != 0) 
            {
                if (true) 
                {
                    ForceDialogue(alarmInnerDialogues);
                    Debug.Log("alarm inner dialogue");
                    if (playerInputHandler.InteractAction.WasPerformedThisFrame()) 
                    {
                        GameObject go = hit.transform.gameObject;
                        Destroy(go, .01f);
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
                    ForceDialogue(ladderLockedInnerDialogue);
                }
                else 
                {
                    ForceDialogue(ladderUnlockedInnerDialogue);
                    if (playerInputHandler.InteractAction.WasPerformedThisFrame())
                    {
                        ClimbLadder();
                    }
                }
            }

            //Level 2 /////////////////////////////////////////////////////////////


            //Helper2
            if (((1 << layer) & helper2Layer) != 0)
            {
                if (!hasAccessToBrdige && !hasValuable)
                    ForceDialogue(helper2ValuableDialogue);
                else if (!hasAccessToBrdige)
                    ForceDialogue(helper2ValuableDialogue);
            }

            //Valuable
            if (((1 << layer) & valuableLayer) != 0)
            {
                if (!hasValuable)
                {
                    ForceDialogue(valuableDialogue);

                    if(playerInputHandler.InteractAction.WasCompletedThisFrame()) 
                    {
                        GameObject go = hit.transform.gameObject;
                        hasValuable = true;
                        gotWeapon.Invoke();
                        ForceDialogue(valuableCollectedDialogue);
                        Destroy(go, .01f);
                    }
                }
                else 
                {
                    ForceDialogue(hasValuableDialogue);
                }
            }

            //Rioter
            if (((1 << layer) & rioterLayer) != 0)
            {
                if (!hasAccessToBrdige)
                {
                    ForceDialogue(rioterDialogue);
                }

                if (hasValuable && !hasAccessToBrdige && playerInputHandler.InteractAction.WasCompletedThisFrame())
                {
                    FadeImageGO.SetActive(true);

                    ForceDialogue(rioterChantDialogue);

                    hasAccessToBrdige = true;
                    GameManager.Instance.BridgeBlocker.SetActive(false);

                    Debug.Log("RiotSTarted");
                    riotStarted.Invoke(transform.position);

                    ForceDialogue(rioterChantDialogue);

                    return;
                }
                else 
                {
                    if (!hasAccessToBrdige)
                    {
                        ForceDialogue(rioterDialogue);
                    }
                }
                
            }

            //Bridge
            if (((1 << layer) & bridgeLayer) != 0) 
            {
                if (!hasAccessToBrdige) 
                {
                    ForceDialogue(bridgeLockedDialogues);
                }
            }
        }

        if (hasAccessToBrdige) 
        {
            float distance = Vector3.Distance(transform.position, GameManager.Instance.EndingTransform.position);
            float normalize = Mathf.Clamp01(distance / maxEndingDist);
            float inverse = 1 - normalize;
            FadeImage.color = Color.Lerp(Color.clear, endingColor, inverse);

            if (inverse > .01f)
                GameManager.Instance.canGetCaught = false;
            else
                GameManager.Instance.canGetCaught = true;

            if (inverse > .9f) 
            {
                FadeImage.color = Color.Lerp(Color.clear, endingColor, inverse += Time.deltaTime);
                LoadScene("TitleScreen");
            }
        }

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

    public void LostGame() 
    {
        FadeImageGO.SetActive(true);
        FadeImage.color = Color.clear;
        FadeImage.DOColor(Color.red, 1f)
        .OnComplete(() =>
        {
            LoadScene("TitleScreen");
        });
    }

    #region Text Management
    void ForceDialogue(string[] dialogues)
    {
        if (lastDialogues == dialogues)
            return;

        lastDialogues = dialogues;


        if (textDisappearRoutine != null)
            StopCoroutine(textDisappearRoutine);
        if (textAppearRoutine != null)
            StopCoroutine(textAppearRoutine);

        int i = Random.Range(0, dialogues.Length);
        string text = dialogues[i];
        textAppearRoutine = StartCoroutine(TextAppearCoroutine(text));
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
        textDisappearRoutine = StartCoroutine(TextDissapearCoroutine(text));
        textAppearRoutine = null;
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

