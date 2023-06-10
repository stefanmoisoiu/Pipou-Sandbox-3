using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class Currency : MonoBehaviour
{
    public static Action<string> onCollect;
    
    const string Glyphs= "abcdefghijklmnopqrstuvwxyz0123456789";
    private const int charAmount = 12;
    [SerializeField] private string id;


    private void Start()
    {
        CheckDisable();
        CloudCurrency.onCurrencyIdUpdated += CheckDisable;
    }

    [Button]
    private void GenerateID()
    {
        string str = "";
        for(int i=0; i<charAmount; i++)
        {
            str += Glyphs[Random.Range(0, Glyphs.Length)];
        }

        id = str;
    }
    
    public void Collect()
    {
        onCollect?.Invoke(id);
        gameObject.SetActive(false);
    }

    private void CheckDisable()
    {
        if (!gameObject.activeSelf || CloudCurrency.CollectedCurrency == null) return;
        if (CloudCurrency.CollectedCurrency.Contains(id)) gameObject.SetActive(false);
    }
}
