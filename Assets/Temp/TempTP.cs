using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempTP : MonoBehaviour
{
    [SerializeField] private Transform tpPos;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) other.transform.position = tpPos.position;
    }
}
