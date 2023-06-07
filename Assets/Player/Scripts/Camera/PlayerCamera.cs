using System;
using Cinemachine;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.Netcode;

public class PlayerCamera : NetworkBehaviour
{
        [FoldoutGroup("Properties")] [SerializeField]
        private float sensitivity = 1,minHeadAngle = 90,maxHeadAngle = 90,rotLerpSpeed = 10;

        
        [FoldoutGroup("References")] [SerializeField]
        private Transform orientation,head,headCam;

        public static Vector2 TargetRotation { get; private set; }
        public static void SetTargetRotation(Vector2 value) => TargetRotation = value;
        public static bool CanLook { get; private set; }
        public static void SetCanLook(bool value) => CanLook = value;
        private void Start()
        {
                CanLook = true;
                if (IsOwner)
                {
                        Cursor.visible = false;
                        Cursor.lockState = CursorLockMode.Locked;
                        Camera.main.transform.position = headCam.position;
                }
                else
                {
                        headCam.GetComponent<CinemachineVirtualCamera>().Priority = -100;
                }
        }

        private void Update()
        {
                if(IsOwner && CanLook) Look();
        }

        private void Look()
        {
                TargetRotation += new Vector2(
                        InputManager.LookInput.x * sensitivity / 5, 
                        Mathf.Clamp(InputManager.LookInput.y * sensitivity / 5,-minHeadAngle, maxHeadAngle));
                orientation.localRotation = Quaternion.Lerp(orientation.localRotation,Quaternion.Euler(new Vector3(0, TargetRotation.x, 0)),rotLerpSpeed * Time.deltaTime);
                head.localRotation = Quaternion.Lerp(head.localRotation,Quaternion.Euler(new Vector3(-TargetRotation.y, 0, 0)),rotLerpSpeed * Time.deltaTime);
        }
        private Vector3 ClampVector(Vector3 direction, Vector3 center, float maxAngle){
 
 
                float angle = Vector3.Angle(center, direction);
                if(angle > maxAngle){
 
                        direction.Normalize();
                        center.Normalize();
                        Vector3 rotation = (direction - center) / angle;
                        return (rotation * maxAngle) + center;
 
                }
 
                return direction;
 
        }
}