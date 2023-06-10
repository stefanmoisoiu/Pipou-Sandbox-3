using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public static class LobbyManager
{
        public static Lobby JoinedLobby;

        public static async Task<bool> CreateLobby(lobbyPlayerInfo lobbyPlayerInfo,int maxPlayers, string relayJoinCode,bool isPrivate = false,float waitTimeSeconds = 15)
        {
                try
                {
                        CreateLobbyOptions createLobbyOptions = new();
                        createLobbyOptions.IsPrivate = isPrivate;
                        createLobbyOptions.Data = new()
                        {
                                { "RelayJoinCode", new DataObject(DataObject.VisibilityOptions.Member, value: relayJoinCode) },
                        };

                        Lobby lobby = await Lobbies.Instance.CreateLobbyAsync($"Partie de {lobbyPlayerInfo.PlayerName}", maxPlayers,
                                createLobbyOptions);
                        JoinedLobby = lobby;
                        Debug.Log($"Created Lobby with join code :{JoinedLobby.LobbyCode}");

                        
                        return true;
                }
                catch (LobbyServiceException e)
                {
                        Debug.LogError($"Create Lobby Error : {e}");
                        return false;
                }
        }



        public static async Task<bool> JoinLobbyById(lobbyPlayerInfo lobbyPlayerInfo, string lobbyId)
        {
                try
                {
                        JoinLobbyByIdOptions options = new() { Player = GetPlayer(lobbyPlayerInfo) };
                        JoinedLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId,options);
                        return true;
                }
                catch(LobbyServiceException e)
                {
                        Debug.LogError($"Join Lobby Error : {e}");
                        return false;
                }
        }
        public static async Task<bool> JoinLobbyByCode(lobbyPlayerInfo lobbyPlayerInfo,string lobbyCode)
        {
                try
                {
                        JoinLobbyByCodeOptions options = new() { Player = GetPlayer(lobbyPlayerInfo) };
                        JoinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode,options);
                        return true;
                }
                catch(LobbyServiceException e)
                {
                        Debug.LogError($"Join Lobby Error : {e}");
                        return false;
                }
        }
        public static async Task<bool> DisconnectFromLobby()
        {
                Debug.Log("Disconnecting from lobby");
                if (JoinedLobby == null) return false;
                try
                {
                        string playerId = AuthenticationService.Instance.PlayerId;
                        await Lobbies.Instance.RemovePlayerAsync(JoinedLobby.Id, playerId);
                        JoinedLobby = null;
                        return true;
                }
                catch(LobbyServiceException e)
                {
                        Debug.LogError($"Failed to leave lobby : {e}");
                        return false;
                }
        }
        
        
        private static Player GetPlayer(lobbyPlayerInfo lobbyPlayerInfo)
        {
                return new()
                {
                        Data = new()
                        {
                                {
                                        "PlayerName",
                                        new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, lobbyPlayerInfo.PlayerName)
                                }
                        }
                };
        }
        public struct lobbyPlayerInfo
        {
                public string PlayerName;
                public lobbyPlayerInfo(string playerName)
                {
                        PlayerName = playerName;
                }
        }
}