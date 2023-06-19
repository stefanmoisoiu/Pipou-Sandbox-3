using Unity.Netcode;
using UnityEngine;

public class SItem : NetworkBehaviour
{
        [SerializeField] private GameObject sleigh;
        [SerializeField] private Rigidbody playerRb;
        [SerializeField] private PlayerSlideCalculator.SlideDampingProperties sleighDampingProperties;

        public static bool Sleighing { get; private set; }

        public void Select()
        {
                Sleighing = true;
        }

        public void UpdateSelected()
        {
                PlayerSlideCalculator.ApplySlideDamping(playerRb.velocity,sleighDampingProperties);
        }
        public void Deselect()
        {
                Sleighing = false;
                playerRb.angularVelocity = Vector3.zero;
                playerRb.MoveRotation(Quaternion.identity);
                PDamping.ResetDamping();
        }
}