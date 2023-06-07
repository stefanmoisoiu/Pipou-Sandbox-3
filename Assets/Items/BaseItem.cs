using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class BaseItem : MonoBehaviour,IPlayerItem
{
    [FormerlySerializedAs("previewSprite")] [SerializeField] private Sprite uiPreviewSprite;
    [FoldoutGroup("Events")]public UnityEvent onSelect, onDeselect, whileSelected, onDelete;
    Sprite IPlayerItem.PreviewSprite => uiPreviewSprite;


    public virtual void Select()
    {
        onSelect?.Invoke();
    }

    public virtual void UpdateSelected()
    {
        whileSelected?.Invoke();
    }

    public virtual void Deselect()
    {
        onDeselect?.Invoke();
    }

    public void Delete()
    {
        onDelete?.Invoke();
    }
}
