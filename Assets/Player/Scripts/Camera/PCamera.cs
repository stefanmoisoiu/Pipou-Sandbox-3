using System;
using Cinemachine;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.Netcode;

public class PCamera : NetworkBehaviour
{
        [FoldoutGroup("Properties")] [SerializeField]
        private float sensitivity = 1,minHeadAngle = 90,maxHeadAngle = 90,rotLerpSpeed = 10;

        
        [FoldoutGroup("References")] [SerializeField]
        private Transform orientation,head,headCam;

        [FoldoutGroup("References")] [SerializeField]
        private CinemachineVirtualCamera fpsCam;
        [FoldoutGroup("References")] [SerializeField]
        private CinemachineFreeLook tpsCam;

        public enum CamType {FPSCam,TPSCam}

        public static CamType Cam { get; private set; }
        public static void SetCurrentCam(CamType camType) => Cam = camType;
        public static Vector2 TargetRotation { get; private set; }
        public static void SetTargetRotation(Vector2 value) => TargetRotation = value;
        public static bool CanLook { get; private set; }
        public static void SetCanLook(bool value) => CanLook = value;
        private void Start()
        {
                if (IsOwner)
                {
                        CanLook = true;
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = true;
                        Camera.main.transform.position = headCam.position;
                }
        }

        private void Update()
        {
                if (!IsOwner) return;
                switch (Cam)
                {
                        case CamType.FPSCam:
                                fpsCam.Priority = 10;
                                tpsCam.Priority = -100;
                                if(CanLook) FPSLook();
                                break;
                        case CamType.TPSCam:
                                fpsCam.Priority = -100;
                                tpsCam.Priority = 10;
                                break;
                }
        }

        private void FPSLook()
        {
                TargetRotation = new Vector2(
                        TargetRotation.x + InputManager.LookInput.x * sensitivity / 5, 
                        Mathf.Clamp(TargetRotation.y + InputManager.LookInput.y * sensitivity / 5,-minHeadAngle, maxHeadAngle));
                
                orientation.localRotation = Quaternion.Lerp(
                        orientation.localRotation,
                        Quaternion.Euler(new Vector3(0, TargetRotation.x, 0)),
                        rotLerpSpeed * Time.deltaTime);
                
                head.localRotation = Quaternion.Lerp(
                        head.localRotation,
                        Quaternion.Euler(new Vector3(-TargetRotation.y, 0, 0)),
                        rotLerpSpeed * Time.deltaTime);
        }
}