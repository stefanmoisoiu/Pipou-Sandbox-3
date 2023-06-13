using System;
using UnityEngine;

public class SkinShopItem : ShopItem
{
        [SerializeField] private int skinMatIndex;

        private void Start()
        {
                onBuy += OnBuy;
        }
        private void OnBuy()
        {
                PSkin.Instance.AddSkin(skinMatIndex);
        }
}