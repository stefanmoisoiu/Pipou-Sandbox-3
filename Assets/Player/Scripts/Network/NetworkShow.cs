using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkShow : NetworkBehaviour
{
    [SerializeField] private ShowProperty[] showProperties;

    private void Start()
    {
        foreach (ShowProperty showProperty in showProperties)
        {
            if (IsOwner)
            {
                showProperty.target.SetActive(showProperty.showAsOwner);
            }
            else
            {
                showProperty.target.SetActive(showProperty.showAsOther);
            }
        }
    }

    [Serializable]
    public class ShowProperty
    {
        public GameObject target;
        public bool showAsOwner=true,showAsOther=true;
    }
}
