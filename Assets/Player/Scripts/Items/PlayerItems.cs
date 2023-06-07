using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PlayerItems : NetworkBehaviour
{
    [SerializeField] private BaseItem testItem;
    [SerializeField] private BaseItem testItem2;
    [SerializeField] private BaseItem testItem3;
    
    [SerializeField] private UIItem[] uiItems;
    private List<GameObject> gItems;
    private List<IPlayerItem> items;
    
    private int selectedItemIndex = -1;
    public static Action onSelect, onDeselect;
    private void Start()
    {
        if (!IsOwner) return;

        items = new();
        for(int i = 0;i < uiItems.Length; i ++)
        {
            items.Add(null);
        }
        if(testItem2 != null) AddItemAtEnd(testItem2);
        if(testItem != null) AddItemAtEnd(testItem);
        if(testItem3 != null) AddItemAtEnd(testItem3);
        
        InputManager.onSelectItem += SelectItem;
        ConveyorBelt.onStartUsing += DeselectItem;
    }

    private void Update()
    {
        if(selectedItemIndex != -1) items[selectedItemIndex].UpdateSelected();
    }

    public void SelectItem(int index)
    {
        if (selectedItemIndex != -1)
        {
            uiItems[selectedItemIndex].UIDeselected();
            items[selectedItemIndex].Deselect();
            if (selectedItemIndex == index || items[index] == null)
            {
                selectedItemIndex = -1;
                return;
            }
        }
        else if (items[index] == null) return;
        
        selectedItemIndex = index;
        uiItems[index].UISelected();
        items[index].Select();
    }

    public void AddItemAtEnd(IPlayerItem item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                SetItem(i, item);
                return;
            }
        }

        SetItem(items.Count - 1, item);
    }
    public void SetItem(int index,IPlayerItem item)
    {
        if (selectedItemIndex == index)
        {
            uiItems[selectedItemIndex].UIDeselected();
            items[selectedItemIndex].Deselect();
            items[selectedItemIndex].Delete();
            Destroy((Component)items[selectedItemIndex]);
            items[selectedItemIndex] = null;
        }

        items[index] = item;
        uiItems[index].SetPreview(item.PreviewSprite);
    }

    public void DeselectItem()
    {
        if (selectedItemIndex == -1) return;
        uiItems[selectedItemIndex].UIDeselected();
        items[selectedItemIndex].Deselect();
        selectedItemIndex = -1;
    }
}
