using System;
using Unity.Netcode;
using UnityEngine;

public class ShopItem : NetworkBehaviour
{
    [SerializeField] private int price;
    [SerializeField] private string currencyValueCloudKey;
    [SerializeField] private string boughtCloudKey;

    public Action onBuy;
    public async void Buy()
    {
        string boughtStr = await CloudSaver.GetData(boughtCloudKey);
        if (!string.IsNullOrEmpty(boughtStr) && bool.Parse(boughtStr)) return;
        
        string currencyAmountStr = await CloudSaver.GetData(currencyValueCloudKey);
        if (string.IsNullOrEmpty(currencyAmountStr)) return;
        int currencyAmount = int.Parse(currencyAmountStr);
        if (price <= currencyAmount)
        {
            currencyAmount -= price;
            await CloudSaver.SaveData(currencyValueCloudKey,currencyAmount);
            await CloudSaver.SaveData(boughtCloudKey,true);
        }
        onBuy?.Invoke();
    }
}