using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NPCDialogController : MonoBehaviour
{
    public static NPCDialogController Instance;
    TMP_Text text;
    GameObject dialogBubbleImage;
    TMP_Text selectionText;
    GameObject selectionBubbleImage;
    TMP_Text SpeakerName;
    PlayerMovement player;
    internal Animator avatarAnimator;
    GameObject DialogHolder;

    [SerializeField] float TalkSpeed = .05f;

    // Hardcoded because its universal across all npcs
    private readonly float OpenY = 0f, CloseY = -625f;
    private readonly float Duration = 1f;

    public DialogLine[] lines;
    public object[] TextArguments;
    int pointer = 0;
    bool wasPointerModified = false;

    bool renderSelectionNext = false;
    bool isSelectionActive = false;
    int selectedChoice = 0;
    List<DialogChoice> choices;
    bool ready = false; // talking
    bool end = false;
    NPCData npcData;

    public bool Active { private set; get; } = false;

    string GetFormattedSpeaker()
    {
        string color = npcData.SpeakerGender == Gender.MALE ? "#6495ED" : "#F88379";
        return $"<color={color}>{npcData.SpeakerName}</color>";
    }

    public void SetPointer(int newPointer)
    {
        pointer = newPointer;
        wasPointerModified = true;
    }

    public void Activate(int customPointer = 0)
    {
        Debug.Log("Activate dialog");
        // here we copy current color changing material to avatar renderer so colors of character are same
        try
        {
            avatarAnimator.gameObject.GetComponent<Image>().material.SetColor("_ReplaceColor", GetComponent<Renderer>().material.GetColor("_ReplaceColor"));
        }
        catch
        {
            Debug.Log("Skipped shader part");
        }
        pointer = customPointer;
        if(customPointer > 0)
        {
            SetPointer(customPointer);
        }
        selectedChoice = 0;
        Active = true;
        end = false;
        text.text = "";
        DialogHolder.SetActive(true);
        dialogBubbleImage.transform.localScale = Vector3.one;
        if(player != null) player.SetAllowedToMove(false);
        SpeakerName.text = GetFormattedSpeaker(); // set speaker name. MAX 6 CHARACTERS (for now)
        StartCoroutine(ToggleDialogObject());
    }

    public void EndDialog(string message = null)
    {
        if (message != null) Debug.LogWarning(message);
        end = false;
        Active = false;
        StartCoroutine(ToggleDialogObject());
    }

    IEnumerator ToggleDialogObject()
    {
        Debug.Log("Animating DialogHolder");
        float elapsed = 0f;
        Vector3 a = Active ? new Vector3(0f, CloseY) : new Vector3(0f, OpenY);
        Vector3 b = Active ? new Vector3(0f, OpenY) : new Vector3(0f, CloseY);
        RectTransform dgHolder = DialogHolder.GetComponent<RectTransform>();
        while (elapsed < Duration)
        {
            float t = Mathf.Clamp01(elapsed / Duration);
            float easedT = Mathf.SmoothStep(0f, 1f, t);
            dgHolder.anchoredPosition = Vector3.Lerp(a, b, easedT);

            elapsed += Time.deltaTime;
            yield return null;
        }
        if (Active)
        {
            Debug.Log("Rendering dialog after animation");
            yield return new WaitForSeconds(.2f);
            // setup
            avatarAnimator.SetBool("Annoyed", false);
            avatarAnimator.SetTrigger("Talk");
            Render();
        } else
        {
            Debug.LogWarning("Stopping dialog and hiding Holder");
            DialogHolder.SetActive(false);
            Clear();
            if (player != null) player.SetAllowedToMove(true);
        }
    }

    private void Awake()
    {
        Instance = this;
        npcData = GetComponent<NPCData>();
        if (npcData == null)
        {
            Debug.LogError("No NPCData on GameObject with NPCDialogController. Halting script");
            // fundamentals break, why continue?
            Destroy(this);
            return;
        }
        try
        {
            text = GameObject.Find("SpeechText").GetComponent<TMP_Text>();
            dialogBubbleImage = GameObject.Find("SpeechBubble");
            selectionBubbleImage = GameObject.Find("SelectionBubble");
            selectionText = GameObject.Find("SelectionText").GetComponent<TMP_Text>();
            avatarAnimator = dialogBubbleImage.transform.GetChild(0).gameObject.GetComponent<Animator>();
            DialogHolder = GameObject.Find("Dialog");
            SpeakerName = GameObject.Find("SpeakerName").GetComponent<TMP_Text>();
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("Some components are unavailable for NPCDialogController\n"+ex.Message);
        }
        try
        {
            player = GameObject.Find("player_0").GetComponent<PlayerMovement>();
        } catch
        {
            Debug.LogWarning("Player doesnt exist");
        }
        DialogHolder.SetActive(false);
    }

    void Render()
    {
        selectionBubbleImage.transform.localScale = Vector3.zero;
        selectionText.text = "";
        if(player != null) player.SetAllowedToMove(false); // just in case
        if (end)
        {
            EndDialog();
            return;
        }
        if (!renderSelectionNext) text.text = "";
        DialogLine line = lines[pointer];
        end = line.DontProceed;
        if (renderSelectionNext)
        {
            selectionBubbleImage.transform.localScale = Vector3.one;
            choices = line.choices;
            RenderChoices();
            renderSelectionNext = false;
            isSelectionActive = true;
        }
        else
        {
            string lineText = line.text[Random.Range(0, 3)];
            if(TextArguments != null) lineText = string.Format(lineText, TextArguments);
            StartCoroutine(AnimTalk(lineText));
            renderSelectionNext = line.hasChoices;
        }
    }

    void RenderChoices()
    {
        string output = "";
        int j = 0;
        foreach(DialogChoice choice in choices)
        {
            if (j == selectedChoice) output += "<color=#FFFFFF>";
            output += $"{j+1}. {choice.text}";
            if (j == selectedChoice) output += "</color>";
            output += "\n";
            j++;
        }
        selectionText.text = output;
    }

    void Clear()
    {
        dialogBubbleImage.transform.localScale = Vector3.zero;
        selectionBubbleImage.transform.localScale = Vector3.zero;
        text.text = "";
        selectionText.text = "";
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Clear();
    }

    // Update is called once per frame
    void Update()
    {
        if(Active)
        {
            if(Keyboard.current.spaceKey.wasPressedThisFrame && ready)
            {
                if (isSelectionActive) {
                    choices[selectedChoice].onChosen.Invoke(); // invoke selected action and proceed
                    isSelectionActive = false;
                } 
                if(!renderSelectionNext && !wasPointerModified) pointer++;
                if (pointer < lines.Length)
                {
                    wasPointerModified = false;
                    Render();
                } else
                {
                    EndDialog();
                }
            }
            if (isSelectionActive)
            {
                if (Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame)
                {
                    if (selectedChoice > 0)
                    {
                        selectedChoice--;
                        RenderChoices();
                    }
                }
                if (Keyboard.current.downArrowKey.wasPressedThisFrame || Keyboard.current.sKey.wasPressedThisFrame)
                {
                    if (selectedChoice+1 < choices.Count)
                    {
                        selectedChoice++;
                        RenderChoices();
                    }
                }
            }
        }
    }

    IEnumerator AnimTalk(string t)
    {
        ready = false;
        avatarAnimator.SetTrigger("Talk");
        foreach(char c in t)
        {
            text.text += c;
            yield return new WaitForSeconds(TalkSpeed);
        }
        ready = true;
        avatarAnimator.SetTrigger("Blink");
    }
}
