using System;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class JoinGameList : MonoBehaviour
{
        private bool isRefreshing = false;
        
        [FoldoutGroup("References")] [SerializeField]
        private GameObject gameList;
        [FoldoutGroup("References")] [SerializeField]
        private Transform listLayoutParent;
        [FoldoutGroup("References")] [SerializeField]
        private JoinGameButton gameButtonPrefab;

        private MenuRelayManager _menuRelayManager;

        private void Start()
        {
                _menuRelayManager = GetComponent<MenuRelayManager>();
        }

        public void ShowGameList(bool show)
        {
                gameList.SetActive(show);
                if (show) RefreshGameList();
        }

        public async void RefreshGameList()
        {
                if (isRefreshing) return;
                isRefreshing = true;
                foreach (Transform child in listLayoutParent) {
                        Destroy(child.gameObject);
                }

                QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync();
                foreach (var lobby in lobbies.Results)
                {
                        JoinGameButton button = Instantiate(gameButtonPrefab, listLayoutParent);
                        button.gameNameText.text = lobby.Name;
                        button.playerCountText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
                        button.joinButton.onClick.AddListener(delegate{_menuRelayManager.JoinGame(lobby.Id,NetcodeGameManager.lobbyJoinType.Id);});
                }

                isRefreshing = false;
        }
}