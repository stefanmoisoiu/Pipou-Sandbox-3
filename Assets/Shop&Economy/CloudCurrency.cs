using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class CloudCurrency : MonoBehaviour
{
    [SerializeField] private string currencyIdsCloudKey,currencyValueCloudKey;
    public static string[] CollectedCurrency { get; private set; }
    public static Action onCurrencyIdUpdated;
    public static Action<int> onCurrencyValueUpdated;
    private void Start()
    {
        CheckCollectedCurrency();
        Currency.onCollect += AddCollectedCurrency;
    }

    private async void CheckCollectedCurrency()
    {
        string idsStr = await CloudSaver.GetData(currencyIdsCloudKey);
        if (string.IsNullOrEmpty(idsStr))
        {
            CollectedCurrency = new string[] {};
            return;
        }
        CollectedCurrency = idsStr.Split(",");
        onCurrencyIdUpdated?.Invoke();
    }

    private async void AddCollectedCurrency(string id)
    {
        string idsStr = await CloudSaver.GetData(currencyIdsCloudKey);
        if (!string.IsNullOrEmpty(idsStr))
        {
            string[] ids = idsStr.Split(",");
            if (ids.Contains(id)) return;
            List<string> idList = ids.ToList();
            idList.Add(id);
            
            string newVal = "";
            for (int i = 0; i < idList.Count-1; i++)
            {
                newVal += idList[i] + ",";
            }
            newVal += idList[^1];
            
            await CloudSaver.SaveData(currencyIdsCloudKey,newVal);
        }
        else await CloudSaver.SaveData(currencyIdsCloudKey,id);

        string currencyAmountStr = await CloudSaver.GetData(currencyValueCloudKey);
        
        int currencyAmount = 0;
        if (!string.IsNullOrEmpty(currencyAmountStr))
            currencyAmount = int.Parse(currencyAmountStr);

        currencyAmount++;
        onCurrencyValueUpdated?.Invoke(currencyAmount);
        await CloudSaver.SaveData(currencyValueCloudKey, currencyAmount);
    }
}
