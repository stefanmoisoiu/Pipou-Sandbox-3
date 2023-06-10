using System;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.Utilities;
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

    public static uint CurrentSkin { get; private set; }
    private NetworkVariable<uint> currentNetworkSkin = new (writePerm: NetworkVariableWritePermission.Owner);

    public static Action onStart;
    private async void Start()
    {
        if (!IsOwner) return;
        Instance = this;
        string selectedSkin = await CloudSaver.GetData(selectedSkinCloudKey);
        if (!string.IsNullOrEmpty(selectedSkin)) await SetSkin(int.Parse(selectedSkin));
        onStart?.Invoke();
    }

    private void Update()
    {
        if (!IsOwner && CurrentSkin != currentNetworkSkin.Value)
        {
            CurrentSkin = currentNetworkSkin.Value;
            foreach (Renderer meshRenderer in meshesToChange)
            {
                Material[] mats = meshRenderer.materials;
                mats[0] = skins[currentNetworkSkin.Value];
                meshRenderer.materials = mats;
            }
        }
    }

    public async Task SetSkin(int matIndex)
    {
        foreach (Renderer meshRenderer in meshesToChange)
        {
            Material[] mats = meshRenderer.materials;
            mats[0] = skins[matIndex];
            meshRenderer.materials = mats;
        }

        CurrentSkin = (uint)matIndex;
        currentNetworkSkin.Value = (uint)matIndex;
        await CloudSaver.SaveData(selectedSkinCloudKey, matIndex);
    }
    public Material GetSkinMat(int matIndex)
    {
        return skins[matIndex];
    }
    public Texture2D GetSkinTexture(int spriteIndex)
    {
        return skinSprites[spriteIndex];
    }

    public async void AddSkin(int matIndex)
    {
        int[] availableSkins = await GetAvailableSkins();
        if (availableSkins.Contains(matIndex)) return;
        availableSkins = (int[])availableSkins.Append(matIndex);
        SaveAvailableSkins(availableSkins);
    }
    public async Task<int[]> GetAvailableSkins()
    {
        string availableSkins = await CloudSaver.GetData(skinListCloudKey);
        if (availableSkins == null || availableSkins == "") return new []{0};
        return availableSkins.Split(",").Select(int.Parse).Prepend(0).ToArray();
    }

    public async void SaveAvailableSkins(int[] availableSkins)
    {
        string value = "";
        for (int i = 0; i < availableSkins.Length-1; i++)
        {
            value += availableSkins[i] + ",";
        }
        value += availableSkins;

        await CloudSaver.SaveData(skinListCloudKey, value);
    }
    public async Task<bool> HasSkin(int index) =>  (await GetAvailableSkins()).Contains(index);
}
