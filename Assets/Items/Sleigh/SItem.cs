using Unity.Netcode;
using UnityEngine;

public class SItem : NetworkBehaviour
{
        [SerializeField] private GameObject sleigh;
        [SerializeField] private Rigidbody playerRb;
        [SerializeField] private PlayerSlideCalculator.SlideDampingProperties sleighDampingProperties;
        private NetworkVariable<bool> enableSleigh = new(writePerm: NetworkVariableWritePermission.Owner);

        public static bool Sleighing { get; private set; }

        private void Update()
        {
                if(!IsOwner) sleigh.SetActive(enableSleigh.Value);
        }

        public void Select()
        {
                Sleighing = true;
                enableSleigh.Value = true;
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
                enableSleigh.Value = false;
        }
}