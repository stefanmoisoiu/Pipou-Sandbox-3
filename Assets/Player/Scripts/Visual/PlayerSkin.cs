using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class PlayerSkin : NetworkBehaviour
{
    public static PlayerSkin Instance;
    
    [SerializeField] private Renderer[] meshesToChange;
    [SerializeField] private Material[] skins;
    [SerializeField] private Texture2D[] skinSprites;
    [SerializeField] private string selectedSkinCloudKey;
    [SerializeField] private string skinListCloudKey;

    private uint currentSkin = 0;
    private NetworkVariable<uint> currentNetworkSkin = new (writePerm: NetworkVariableWritePermission.Owner);

    private async void Start()
    {
        if (!IsOwner) return;
        Instance = this;
        string selectedSkin = await CloudSaver.GetData(selectedSkinCloudKey);
        if (selectedSkin == "") return;
        SetSkin(int.Parse(selectedSkin));
    }

    private void Update()
    {
        if (currentSkin != currentNetworkSkin.Value)
        {
            currentSkin = currentNetworkSkin.Value;
            foreach (Renderer meshRenderer in meshesToChange)
                meshRenderer.materials[0] = skins[currentNetworkSkin.Value];
        }
    }

    public async void SetSkin(int index)
    {
        foreach (Renderer meshRenderer in meshesToChange)
            meshRenderer.materials[0] = skins[index];
        currentNetworkSkin.Value = (uint)index;
        await CloudSaver.SaveData(selectedSkinCloudKey, index);
    }
    public Material GetSkinMat(int index)
    {
        return skins[index];
    }
    public Texture2D GetSkinTextures(int index)
    {
        return skinSprites[index];
    }

    public async Task<bool> HasSkin(int index)
    {
        if (index == 0) return true;
        string skinList = await CloudSaver.GetData(skinListCloudKey);
        return skinList.Split(",").Contains(index.ToString());
    }
}
