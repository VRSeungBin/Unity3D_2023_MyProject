using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DialogSystem : MonoBehaviour
{
    [SerializeField]
    private SpeakerUI[] speakers;
    [SerializeField]
    private DialogData[] dialogs;
    [SerializeField]
    private bool DialogInit = true;
    [SerializeField]
    private bool dialogsDB = false;

    public int currentDialogIndex = -1;
    public int currentSpeakerIndex = 0;
    public float typingSpeed = 0.1f;
    private bool isTypingEffect = false;

    public Entity_Dialogue entity_dialogue;

    private void Awake()
    {
        SetAllClose();
        if (dialogsDB)
        {
            Array.Clear(dialogs, 0, dialogs.Length);
            Array.Resize(ref dialogs, entity_dialogue.sheets[0].list.Count);

            int ArrayCursor = 0;

            foreach(Entity_Dialogue.Param param in entity_dialogue.sheets[0].list)
            {
                dialogs[ArrayCursor].index = param.index;
                dialogs[ArrayCursor].speakerUIindex = param.speakerUIindex;
                dialogs[ArrayCursor].name = param.name;
                dialogs[ArrayCursor].dialogue = param.dialogue;
                dialogs[ArrayCursor].characterPath = param.characterPath;
                dialogs[ArrayCursor].tweenType = param.tweenType;
                dialogs[ArrayCursor].nextindex = param.nextindex;

                ArrayCursor += 1;
            }
        }
    }

    public bool UpdateDialog(int currentIndex , bool InitType)
    {
        if(DialogInit == true && InitType == true)
        {
            SetAllClose();
            SetNextDialog(currentIndex);
            DialogInit = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (isTypingEffect == true)
            {
                isTypingEffect = false;
                StopCoroutine("OnTypingText");
                speakers[currentSpeakerIndex].objectArrow.SetActive(true);
                return false;
            }

            if (dialogs[currentDialogIndex].nextindex != -100)
            {
                SetNextDialog(dialogs[currentDialogIndex].nextindex);
            }
            else
            {
                SetAllClose();
                DialogInit = true;
                return true;
            }
        }
        return false;
    }

    private void SetActiveObjects(SpeakerUI speaker, bool visible)
    {
        speaker.imageDialog.gameObject.SetActive(visible);
        speaker.textName.gameObject.SetActive(visible);
        speaker.textDialogue.gameObject.SetActive(visible);

        speaker.objectArrow.SetActive(false);

        Color color = speaker.imgCharacter.color;
        if (visible)
        {
            color.a = 1;
        }
        else
        {
            color.a = 0.2f;
        }
        speaker.imgCharacter.color = color;
    }

    private void SetAllClose()
    {
        for (int i = 0; i < speakers.Length; ++i)
        {
            SetActiveObjects(speakers[i], false);
        }
    }

    private void SetNextDialog(int currentIndex)
    {
        SetAllClose();
        currentDialogIndex = currentIndex;
        currentSpeakerIndex = dialogs[currentSpeakerIndex].speakerUIindex;
        SetActiveObjects(speakers[currentSpeakerIndex], true);

        speakers[currentSpeakerIndex].textName.text = dialogs[currentDialogIndex].name;
        StartCoroutine("OnTypingText");
    }

    private IEnumerator OnTypingText()
    {
        int index = 0;
        isTypingEffect = true;

        if (dialogs[currentDialogIndex].characterPath != "None")
        {
            speakers[currentSpeakerIndex].imgCharacter = (Image)Resources.Load(dialogs[currentDialogIndex].characterPath);
        }

        while (index < dialogs[currentDialogIndex].dialogue.Length + 1)
        {
            speakers[currentSpeakerIndex].textDialogue.text = dialogs[currentDialogIndex].dialogue.Substring(0, index);
            index++;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTypingEffect = false;

        speakers[currentSpeakerIndex].objectArrow.SetActive(true);
    }
}

[System.Serializable]
public struct SpeakerUI
{
    public Image imgCharacter;
    public Image imageDialog;
    public Text textName;
    public Text textDialogue;
    public GameObject objectArrow;
}

[System.Serializable]
public struct DialogData
{
    public int index;
    public int speakerUIindex;
    public string name;
    public string dialogue;
    public string characterPath;
    public int tweenType;
    public int nextindex;
}

