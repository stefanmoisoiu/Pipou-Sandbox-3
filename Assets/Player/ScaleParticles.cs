using System;
using UnityEngine;

public class ScaleParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;

    private void Start()
    {
        ParticleSystem.MainModule main = particleSystem.main;
        main.scalingMode = ParticleSystemScalingMode.Hierarchy;
    }
}