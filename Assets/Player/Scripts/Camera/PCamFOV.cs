using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PCamFOV : NetworkBehaviour
{
        [SerializeField] private CinemachineVirtualCamera cam;
        private static readonly List<IFOV> AddedFOVList = new();
        private float startCamFOV;

        
        public static void AddFOV(IFOV fov) => AddedFOVList.Add(fov);

        private void Start()
        {
                if (!IsOwner) return;
                startCamFOV = cam.m_Lens.FieldOfView;
        }

        private void Update()
        {
                if (!IsOwner) return;
                float fovSum = 0;
                for (int i = 0;i < AddedFOVList.Count; i ++)
                {
                        AddedFOVList[i].Update();
                        if (AddedFOVList[i].Finished()) AddedFOVList.RemoveAt(i);
                        else fovSum += AddedFOVList[i].Get();
                }
                cam.m_Lens.FieldOfView = startCamFOV + fovSum;
        }
        [Serializable]
        public class FadingFOV:IFOV
        {
                [SerializeField] private float startAmount,duration;
                [SerializeField] private AnimationCurve easeCurve;
                private float advancement;

                public static FadingFOV Copy(FadingFOV fadingFOV)
                {
                        FadingFOV copy = new();
                        copy.startAmount = fadingFOV.startAmount;
                        copy.duration = fadingFOV.duration;
                        copy.easeCurve = fadingFOV.easeCurve;
                        copy.advancement = fadingFOV.advancement;
                        return copy;
                }
                public float Get() => Mathf.Lerp(startAmount, 0, easeCurve.Evaluate(advancement));

                public void Update()
                {
                        advancement += Time.deltaTime / duration;
                }

                public bool Finished() => advancement >= 1;
        }
        [Serializable]
        public class ConstantFOV:IFOV
        {
                private float amount;

                [SerializeField] private float startAmount, newValueEaseDuration;
                [SerializeField] private AnimationCurve newValueEaseCurve;
                private float newValueEaseAdvancement = 1;

                public void SetNewAmount(float newAmount)
                {
                        startAmount = newValueEaseAdvancement < 1 ?
                                Mathf.Lerp(startAmount, amount, newValueEaseCurve.Evaluate(newValueEaseAdvancement)) :
                                amount;
                        amount = newAmount;
                        newValueEaseAdvancement = 0;
                }

                public float Get()
                {
                        if (newValueEaseAdvancement < 1 && newValueEaseCurve != null)
                                return Mathf.Lerp(startAmount, amount,
                                        newValueEaseCurve.Evaluate(newValueEaseAdvancement));
                        return amount;
                }

                public void Update()
                {
                        if (newValueEaseAdvancement < 1)
                                newValueEaseAdvancement += Time.deltaTime / newValueEaseDuration;
                }

                public bool Finished() => false;
        }
        public interface IFOV
        {
                public float Get();
                public void Update();
                public bool Finished();
        }
}

