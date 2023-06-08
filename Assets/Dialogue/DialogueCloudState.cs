using System;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class DialogueCloudState
{
    [SerializeField] private string stateKey;
    [SerializeField] private string defaultState;

    public async Task<string> GetState()
    {
        string data = await CloudSaver.GetData(stateKey);
        if (data == null)
        {
            await SetState(defaultState);
            return defaultState;
        }
        return data;
    }
    public async Task SetState(object stateValue) => await CloudSaver.SaveData(stateKey, stateValue);
    public async Task<bool> IsInState(string stateValue) => await GetState() == stateValue;
}