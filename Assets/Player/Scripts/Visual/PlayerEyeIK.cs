using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerEyeIK : NetworkBehaviour
{
    [SerializeField] private Transform eyeTarget;
}
