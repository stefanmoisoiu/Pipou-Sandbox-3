using UnityEngine;

public class DialogueReset : MonoBehaviour
{
    [SerializeField] private string[] dialoguesToReset;
    
    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("RESET DIALOGUE !!!");
            Debug.Log("RESET DIALOGUE !!!");
            Debug.Log("RESET DIALOGUE !!!");
            foreach (string dialogue in dialoguesToReset) await CloudSaver.SaveData(dialogue, "");
        }
    }
}
