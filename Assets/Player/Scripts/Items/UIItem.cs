using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIItem : MonoBehaviour
{
        [SerializeField] private Image previewImage;
        
        [SerializeField] private UnityEvent onSelect, onDeselect;

        public void UISelected()
        {
                onSelect?.Invoke();
        }

        public void UIDeselected()
        {
                onDeselect?.Invoke();
        }

        public void SetPreview(Sprite previewSprite)
        {
                previewImage.sprite = previewSprite;
                previewImage.enabled = previewSprite != null;
        }
}