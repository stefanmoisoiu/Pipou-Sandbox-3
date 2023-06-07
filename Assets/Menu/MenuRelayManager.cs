using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class MenuRelayManager : MonoBehaviour
{
    
    
    [FoldoutGroup("References")] [SerializeField]
    private TMP_InputField nameInput,joinCodeInput;
    
    [BoxGroup("References/Exception")] [SerializeField]
    private Transform exceptionPrefabParent;
    [BoxGroup("References/Exception")] [SerializeField]
    private GameObject exceptionPrefab;

    [FoldoutGroup("Events")] public UnityEvent onCreate,onJoin,onAny;

    

    public async void CreateGame()
    {
        if (LobbyManager.JoinedLobby != null) return;
        if (!CheckName()) return;
        
        bool succeeded = await NetcodeGameManager.Instance.CreateGame(new LobbyManager.lobbyPlayerInfo(playerName:nameInput.text),delegate(string reason) { LogException(reason); });
        if (!succeeded) return;
        
        onCreate?.Invoke();
        onAny?.Invoke();
    }

    public void JoinGameWithInputCode() => JoinGame(joinCodeInput.text.ToUpper(),NetcodeGameManager.lobbyJoinType.Code);
    public async void JoinGame(string value,NetcodeGameManager.lobbyJoinType joinType)
    {
        if (LobbyManager.JoinedLobby != null) return;
        if (!CheckName()) return;
        if (joinType == NetcodeGameManager.lobbyJoinType.Code && !CheckLobbyCode(value)) return;
        
        bool succeeded = await NetcodeGameManager.Instance.JoinGame(new LobbyManager.lobbyPlayerInfo(playerName:nameInput.text),value,joinType,delegate(string reason) { LogException(reason); });
        if (!succeeded) return;
        
        onJoin?.Invoke();
        onAny?.Invoke();
    }
    private bool CheckName()
    {
        if (nameInput.text.Length < 3)
        {
            LogException("Nom trop court");
            return false;
        }
        if (nameInput.text.Length > 15)
        {
            LogException("Nom trop long");
            return false;
        }

        return true;
    }

    private bool CheckLobbyCode(string lobbyCode)
    {
        if (lobbyCode.Length <= 0)
        {
            LogException("Le code du lobby est vide");
            return false;
        }

        return true;
    }
    private void LogException(string e)
    {
        GameObject expetionInstance = Instantiate(exceptionPrefab, exceptionPrefabParent);
        expetionInstance.GetComponent<TMP_Text>().text = e;
    }
}
