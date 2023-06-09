using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerBounds : NetworkBehaviour
{
    [SerializeField] private float yBound = -100;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Vector3 respawnPos = new Vector3(0,5,0);
    
    
    private void Update()
    {
        if (transform.root.position.y < yBound)
        {
            rb.position = respawnPos;
            rb.velocity = Vector3.zero;
            Debug.Log("Teleported back in bounds player");
        }
    }
}
