using System;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class DialoguePlayer : MonoBehaviour
{
    [SerializeField] private DialoguePlayerProperty[] properties;
    private void Start()
    {
        for (int i = 0; i < properties.Length; i++)
        {
            CinemachineVirtualCamera cam = properties[i].dialogueCam;
            if (cam != null)
            {
                cam.Priority = -1;
                properties[i].onFinished.AddListener(() => cam.Priority = -1);
            }
        }
    }

    public void PlayDialogue(int index)
    {
        if (properties[index].usedOneShot) return;
        if(properties[index].oneShot) properties[index].usedOneShot = true;
        if (properties[index].dialogueCam != null) properties[index].dialogueCam.Priority = 200;

        for (int i = 0; i < properties[index].dialogueEvents.Length; i++)
        {
            if(properties[index].dialogueEvents[i].addedEvents) continue;
            DialogueManager.Instance.TextAnimator.onEvent += properties[index].dialogueEvents[i].CheckEvent;
            properties[index].dialogueEvents[i].addedEvents = true;
        }
        
        properties[index].onStarted?.Invoke();
        StartCoroutine(DialogueManager.Instance.PlayDialogue(
                        properties[index].dialogue, 
            () => properties[index].onLineFinished?.Invoke(),
                () => properties[index].onFinished?.Invoke()));
    }
    [Serializable]
    public class DialogueEvent
    {
        public string eventName;
        public UnityEvent @event;
        public bool addedEvents { get; set; }
        public void CheckEvent(string message)
        {
            if(message == eventName) @event?.Invoke();
        }
    }

    [Serializable]
    public class DialoguePlayerProperty
    {
        public ScriptableDialogue dialogue;
        public CinemachineVirtualCamera dialogueCam;
        public bool oneShot;
        [ReadOnly]public bool usedOneShot;
        public DialogueEvent[] dialogueEvents;
        public UnityEvent onStarted,onLineFinished, onFinished;
    }
}
