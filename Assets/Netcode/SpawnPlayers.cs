using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnPlayers : NetworkBehaviour
{
    [SerializeField] private GameObject player;
    
    private void Start()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += CreatePlayer;
            CreatePlayer(OwnerClientId);
        }
    }

    private void CreatePlayer(ulong clientID)
    {
        print($"Creating player for client {clientID}");
        GameObject playerPrefab = Instantiate(player, transform.position, Quaternion.identity);
        playerPrefab.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID);
    }
}
