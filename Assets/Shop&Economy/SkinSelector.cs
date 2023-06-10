using UnityEngine;

public class SkinSelector : MonoBehaviour
{
        [SerializeField] private MeshRenderer meshRenderer;

        private int currentSelectIndex;

        private void Start()
        {
                PlayerSkin.onStart += SetStartPreview;
        }

        private async void SetStartPreview()
        {
                Debug.Log("Set Start Preview");
                int[] availableSkins = await PlayerSkin.Instance.GetAvailableSkins();
                for (int i = 0; i < availableSkins.Length; i++)
                {
                        if (availableSkins[i] == PlayerSkin.CurrentSkin)
                        {
                                currentSelectIndex = i;
                                break;
                        }
                }
                Material[] mats = meshRenderer.materials;
                mats[2] = PlayerSkin.Instance.GetSkinMat((int)PlayerSkin.CurrentSkin);
                meshRenderer.materials = mats;
        }
        public async void SelectSkin(int addedIndex)
        {
                int[] availableSkins = await PlayerSkin.Instance.GetAvailableSkins();
                currentSelectIndex += addedIndex;
                
                if (currentSelectIndex < 0) currentSelectIndex = availableSkins.Length - 1;
                if (currentSelectIndex >= availableSkins.Length) currentSelectIndex = 0;
                
                Material[] mats = meshRenderer.materials;
                mats[2] = PlayerSkin.Instance.GetSkinMat(availableSkins[currentSelectIndex]);
                meshRenderer.materials = mats;
                
                await PlayerSkin.Instance.SetSkin(availableSkins[currentSelectIndex]);
        }
}