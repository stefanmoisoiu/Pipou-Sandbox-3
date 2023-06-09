using System;
using System.Collections;
using Febucci.UI;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : NetworkBehaviour
{
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private Image speakerImage;
    [SerializeField] private TextAnimator textAnimator;
    [SerializeField] private TextAnimatorPlayer player;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource voiceAudioSource;
    [SerializeField] private float skipDialogueSpeed = 3;
    
    
    public static DialogueManager Instance { get; private set; }
    public TextAnimator TextAnimator => textAnimator;
    private bool finishedTyping;
    
    private void Start()
    {
        if (!IsOwner) return;
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        player.onTextShowed.AddListener(delegate { finishedTyping = true;});
    }
    public IEnumerator PlayDialogue(ScriptableDialogue dialogue,Action onLineFinished = null,Action onFinish = null, bool skippable = true,bool autoSkip = false)
    {
        animator.Play("Show Dialogue");
        for (int i = 0; i < dialogue.lines.Length; i++)
        {
            ShowDialogueLine(dialogue, i);
            if(skippable) InputManager.onUse += delegate { SetDialogueSpeed(skipDialogueSpeed); };
            finishedTyping = false;
            while (!finishedTyping)
            {
                yield return null;
            }
            if(skippable) InputManager.onUse -= delegate { SetDialogueSpeed(skipDialogueSpeed); };

            if (!autoSkip)
            {
                bool waitForInput = true;
                InputManager.onUse += delegate { waitForInput = false; };
                yield return new WaitWhile(() => waitForInput);
                InputManager.onUse -= delegate { waitForInput = false; };
            }
            
            SetDialogueSpeed(1);
            animator.Play("Next Dialogue");
            onLineFinished?.Invoke();
        }
        voiceAudioSource.Stop();
        animator.Play("Hide Dialogue");
        yield return null;
        onFinish?.Invoke();
    }

    public void ShowDialogueLine(ScriptableDialogue dialogue,int lineIndex)
    {
        dialogueText.text = dialogue.lines[lineIndex].text;
        speakerText.text = dialogue.lines[lineIndex].speakerName;
        speakerImage.sprite = dialogue.lines[lineIndex].speakerImage;
        if (dialogue.lines[lineIndex].audioType == DialogueLine.AudioType.AudioClip)
        {
            voiceAudioSource.clip = dialogue.lines[lineIndex].audioClip;
            voiceAudioSource.Play();
        }
        else voiceAudioSource.Stop();
    }

    private void SetDialogueSpeed(float speed)
    {
        player.SetTypewriterSpeed(speed);
    }
}
[Serializable]
public class DialogueLine
{
    [Title("Text")]
    [TextArea(10,20)]public string text;
    [Title("Audio")]
    public enum AudioType {None,Typing,AudioClip}

    public AudioType audioType = AudioType.Typing;
    [ShowIf("@audioType == AudioType.Typing")]public AudioClip[] typingSounds;
    [ShowIf("@audioType == AudioType.AudioClip")]public AudioClip audioClip;
    
    [Title("Speaker")]
    [PreviewField]public Sprite speakerImage;
    public string speakerName;
}