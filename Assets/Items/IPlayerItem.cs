using UnityEngine;

public interface IPlayerItem
{
        public Sprite PreviewSprite { get;}
        
        public void Select();
        public void UpdateSelected();
        public void Deselect();
        public void Delete();
}