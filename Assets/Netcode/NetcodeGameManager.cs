using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Unity.Netcode;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.Events;

public class NetcodeGameManager : MonoBehaviour
{
        public static NetcodeGameManager Instance;

        [FoldoutGroup("Properties")] [SerializeField]
        private int maxPlayers = 4,lobbyHeartbeatCooldown = 15;
        
        public static Coroutine Heartbeat;
        
        [FoldoutGroup("Events")] public UnityEvent onCreate,onJoin,onAny;
        
        public enum lobbyJoinType {Code,Id}
        private bool creatingOrJoiningGame;

        public delegate void ErrorCallback(string callbackReason);
        private void Awake()
        {
                if (Instance != null)
                {
                        Destroy(gameObject);
                        return;
                }

                Instance = this;
                DontDestroyOnLoad(this);
        }

        private void OnDestroy() => QuitGame();

        private async void QuitGame()
        {
                await LobbyManager.DisconnectFromLobby();
        }
        public async Task<bool> CreateGame(LobbyManager.lobbyPlayerInfo lobbyPlayerInfo,ErrorCallback errorCallback = null)
        {
                if (creatingOrJoiningGame) return false;
                if (LobbyManager.JoinedLobby != null) return false;
                
                creatingOrJoiningGame = true;
        
                string lobbyCode = await RelayManager.CreateRelay(maxPlayers);
                if (lobbyCode == null)
                {
                        errorCallback?.Invoke("Erreur creation Relay.");
                        creatingOrJoiningGame = false;
                        return false;
                }
        
                bool lobbySucceeded = await LobbyManager.CreateLobby(lobbyPlayerInfo,maxPlayers,lobbyCode);
                if (!lobbySucceeded)
                {
                        errorCallback?.Invoke("Erreur creation Lobby.");
                        creatingOrJoiningGame = false;
                        return false;
                }
                Heartbeat = StartCoroutine(HeartbeatCoroutine());
                
                onCreate?.Invoke();
                onAny?.Invoke();
                creatingOrJoiningGame = false;
                return true;
        }
        public async Task<bool> JoinGame(LobbyManager.lobbyPlayerInfo lobbyPlayerInfo, string value,lobbyJoinType joinType,ErrorCallback errorCallback = null)
        {
                if (creatingOrJoiningGame) return false;
                if (LobbyManager.JoinedLobby != null) return false;
                
                creatingOrJoiningGame = true;
        
                switch (joinType)
                {
                        case lobbyJoinType.Code:
                                bool lobbyCodeSucceeded = await LobbyManager.JoinLobbyByCode(lobbyPlayerInfo,value);
                                if (!lobbyCodeSucceeded)
                                {
                                        errorCallback?.Invoke("Erreur connection Lobby par code.");
                                        creatingOrJoiningGame = false;
                                        return false;
                                }
                                break;
                        case lobbyJoinType.Id:
                                bool lobbyIdSucceeded = await LobbyManager.JoinLobbyById(lobbyPlayerInfo,value);
                                if (!lobbyIdSucceeded)
                                {
                                        errorCallback?.Invoke("Erreur connection Lobby par ID.");
                                        creatingOrJoiningGame = false;
                                        return false;
                                }
                                break;
                }
        
                bool relaySucceeded = await RelayManager.JoinRelay(LobbyManager.JoinedLobby.Data["RelayJoinCode"].Value);
                if (!relaySucceeded)
                {
                        errorCallback?.Invoke("Erreur connection Relay.");
                        creatingOrJoiningGame = false;
                        return false;
                }
        
                onJoin?.Invoke();
                onAny?.Invoke();
                creatingOrJoiningGame = false;
                return true;
        }

        private IEnumerator HeartbeatCoroutine()
        {
                WaitForSeconds waitForSeconds = new WaitForSeconds(lobbyHeartbeatCooldown);
                while (true)
                {

                        yield return waitForSeconds;
                        if (LobbyManager.JoinedLobby == null)
                        {
                                Debug.Log("Stopping Heartbeat");
                                break;
                        }

                        Lobbies.Instance.SendHeartbeatPingAsync(LobbyManager.JoinedLobby.Id);
                        Debug.Log("Received Heartbeat !");
                }
        }
}