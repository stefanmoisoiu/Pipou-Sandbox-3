using UnityEngine;
using UnityEngine.UI;

public class SkinSelector : MonoBehaviour
{
        [SerializeField] private RawImage skinPreview;
        
        private int currentSelectIndex = 0;

        public async void SelectSkin(int addedIndex)
        {
                if (!await PlayerSkin.Instance.HasSkin(currentSelectIndex + addedIndex)) return;
                currentSelectIndex += addedIndex;
                PlayerSkin.Instance.SetSkin(currentSelectIndex);
                skinPreview.texture = PlayerSkin.Instance.GetSkinTextures(currentSelectIndex);
        }
}