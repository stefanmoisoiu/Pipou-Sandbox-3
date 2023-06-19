using UnityEngine;

public class ZoneTitle : MonoBehaviour
{
    [SerializeField] private string title, subtitle;
    [SerializeField] private float titleDuration = 3;
    
    private static string CurrentZoneTitle;

    public void TrySetTitle()
    {
        if (CurrentZoneTitle == title) return;

        CurrentZoneTitle = title;
        TitleManager.Instance.PlayTitle(title,subtitle,titleDuration);
    }
}
