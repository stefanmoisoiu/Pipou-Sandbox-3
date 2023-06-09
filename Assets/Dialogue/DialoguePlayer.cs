using System;
using System.Threading.Tasks;
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

    public async void PlayDialogue(int index)
    {
        if (properties[index].dialogueKey != default)
        {
            string conditionValue = await properties[index].GetState();
            conditionValue ??= "";
            foreach (DialoguePlayerCondition condition in properties[index].conditions)
            {
                switch (condition.condition)
                {
                    case DialoguePlayerCondition.ConditionType.Equal:
                        if (conditionValue != condition.cloudValue) return;
                        break;
                    case DialoguePlayerCondition.ConditionType.Different:
                        if (conditionValue == condition.cloudValue) return;
                        break;
                }
            }
        }
        
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
                        () =>
                        {
                            if (properties[index].setStateOnFinish)
                                properties[index].SetState(properties[index].onFinishState);
                            properties[index].onFinished?.Invoke();
                        },
                        properties[index].skippable,
                        properties[index].autoSkip));
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
        [Title("Info")]
        public ScriptableDialogue dialogue;
        public CinemachineVirtualCamera dialogueCam;
        [Title("Dialogue Flow")]
        public bool skippable = true;
        public bool autoSkip;
        [Title("Cloud & Conditions")]
        public string dialogueKey;
        public DialoguePlayerCondition[] conditions;
        [Title("Events")]
        public bool setStateOnFinish;
        [ShowIf("setStateOnFinish")]public string onFinishState;
        [Space]
        public DialogueEvent[] dialogueEvents;
        public UnityEvent onStarted,onLineFinished, onFinished;

        public async Task<string> GetState() => await CloudSaver.GetData(dialogueKey);
        public async void SetState(string state) => await CloudSaver.SaveData(dialogueKey, state);
    }

    [Serializable]
    public struct DialoguePlayerCondition
    {
        public ConditionType condition;
        public string cloudValue;
        public enum ConditionType {Equal,Different}
    }
}
