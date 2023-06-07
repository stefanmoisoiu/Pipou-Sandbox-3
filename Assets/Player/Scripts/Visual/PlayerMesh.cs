using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class PlayerMesh : NetworkBehaviour
{
    [SerializeField] private bool forceShowMesh = false;
    [SerializeField] private SkinnedMeshRenderer bodyMesh;
    [SerializeField] private GameObject character;
    [SerializeField] private float ownerMoveCharacter = 0.4f;

    private void Start()
    {
        if (IsOwner && !forceShowMesh) bodyMesh.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        if (IsOwner) character.transform.position -= Vector3.forward * ownerMoveCharacter;
    }
}
