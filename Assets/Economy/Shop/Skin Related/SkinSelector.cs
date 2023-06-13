using UnityEngine;

public class SkinSelector : MonoBehaviour
{
        [SerializeField] private MeshRenderer meshRenderer;

        private int currentSelectIndex;

        private void Start()
        {
                SetStartPreview();
                PSkin.onStart += SetStartPreview;
        }

        private async void SetStartPreview()
        {
                if (PSkin.Instance == null) return;
                Debug.Log("Set Start Preview");
                int[] availableSkins = await PSkin.Instance.GetAvailableSkins();
                for (int i = 0; i < availableSkins.Length; i++)
                {
                        if (availableSkins[i] == PSkin.CurrentSkin)
                        {
                                currentSelectIndex = i;
                                break;
                        }
                }
                Material[] mats = meshRenderer.materials;
                mats[2] = PSkin.Instance.GetSkinMat((int)PSkin.CurrentSkin);
                meshRenderer.materials = mats;
        }
        public async void SelectSkin(int addedIndex)
        {
                int[] availableSkins = await PSkin.Instance.GetAvailableSkins();
                currentSelectIndex += addedIndex;
                
                if (currentSelectIndex < 0) currentSelectIndex = availableSkins.Length - 1;
                if (currentSelectIndex >= availableSkins.Length) currentSelectIndex = 0;
                
                Material[] mats = meshRenderer.materials;
                mats[2] = PSkin.Instance.GetSkinMat(availableSkins[currentSelectIndex]);
                meshRenderer.materials = mats;
                
                await PSkin.Instance.SetSkin(availableSkins[currentSelectIndex]);
        }
}