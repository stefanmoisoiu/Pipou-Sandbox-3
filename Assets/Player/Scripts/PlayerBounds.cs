using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerBounds : NetworkBehaviour
{
    [SerializeField] private float yBound = -100;
    
    private void Update()
    {
        if (transform.root.position.y < yBound)
        {
            transform.root.position = Vector3.zero;
            Debug.Log("Teleported back in bounds player");
        }
    }
}
