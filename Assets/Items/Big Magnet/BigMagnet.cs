using System;
using Unity.Netcode;
using UnityEngine;

public class BigMagnet : NetworkBehaviour
{
    [SerializeField] private InputBind startPullBind,stopPullBind,startPushBind,stopPushBind;
    [SerializeField] private float force;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private ParticleSystem pullParticles,pushParticles;
    [SerializeField] private GameObject pullCollider,pushCollider;
    private uint currentMagnetAction;
    private NetworkVariable<uint> netMagnetAction = new(writePerm: NetworkVariableWritePermission.Owner);
    
    // Magnet Actions : None,Pull,Push
    public void SetMagnet(int magnetAction)
    {
        switch (magnetAction)
        {
            case 0:
                pullParticles.Stop();
                pushParticles.Stop();
                pullCollider.SetActive(false);
                pushCollider.SetActive(false);
                break;
            case 1:
                pullParticles.Play();
                pushParticles.Stop();
                pullCollider.SetActive(true);
                pushCollider.SetActive(false);
                break;
            case 2:
                pullParticles.Stop();
                pushParticles.Play();
                pullCollider.SetActive(false);
                pushCollider.SetActive(true);
                break;
        }

        currentMagnetAction = (uint)magnetAction;
        if(IsOwner) netMagnetAction.Value = (uint)magnetAction;
    }

    private void Update()
    {
        if (!IsOwner)
        {
            if (currentMagnetAction != netMagnetAction.Value) SetMagnet((int)netMagnetAction.Value);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsOwner || !other.CompareTag("Big Magnet")) return;
        if (other.gameObject == pullCollider || other.gameObject == pushCollider) return;
        rb.AddForce(other.transform.forward * force,ForceMode.Force);
    }

    public void Selected()
    {
        startPullBind.Bind();
        stopPullBind.Bind();
        startPushBind.Bind();
        stopPushBind.Bind();
    }

    public void Deselected()
    {
        startPullBind.UnBind();
        stopPullBind.UnBind();
        startPushBind.UnBind();
        stopPushBind.UnBind();
    }
}
