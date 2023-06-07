using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerStateManager : NetworkBehaviour
{
        // public enum MovementState {Normal,Crouching,Sliding,Diving,WallClimbing}
        //
        // public static MovementState CurrentMovementState;
        //
        // [SerializeField] private PlayerMovementState[] playerMovementStates;
        // private static Dictionary<MovementState, IPlayerMovement> playerMovementDict;
        //
        // public static void ApplyPlayerMovementAction
        //
        // [Button("Generate Movement Dict")]
        // private void GenerateMovementDict()
        // {
        //         playerMovementDict = new();
        //         foreach (var playerMovementState in playerMovementStates)
        //         {
        //                 try
        //                 {
        //                         playerMovementDict.Add(playerMovementState.movementState,
        //                                 (IPlayerMovement)playerMovementState.movement);
        //                 }
        //                 catch
        //                 {
        //                         Debug.LogError("Failed to generate.");
        //                 }
        //         }
        //         Debug.Log("Finished generating movement dict.");
        // }
        //
        // [System.Serializable]
        // private class PlayerMovementState
        // {
        //         public Component movement;
        //         public MovementState movementState;
        // }
        //
        // [System.Serializable]
        // public class PlayerMovementAction
        // {
        //         public MovementState[] statesToStop;
        //         public bool allowMovement = true;
        //         
        //         [HorizontalGroup("Damping")] public bool setDamping = true;
        //         [HorizontalGroup("Damping")] [ShowIf("setDamping")] public bool setDamping = true;
        //         
        //         [HorizontalGroup("Height")] public bool setHeight = true;
        //         [HorizontalGroup("Height")] [ShowIf("setHeight")] public PlayerHeight.HeightType heightToSet = PlayerHeight.HeightType.Normal;
        // }
}