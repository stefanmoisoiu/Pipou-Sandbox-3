using TMPro;
using Unity.Netcode;
using UnityEngine;

public class TitleManager : NetworkBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text subtitleText;
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip titleClip;
    public static TitleManager Instance { get; private set; }
    

    private void Start()
    {
        if (!IsOwner)
        {
            return;
        }

        Instance = this;
    }

    public void PlayTitle(string title, string subtitle,float duration)
    {
        animator.Play(titleClip.name);
        animator.speed = titleClip.length / duration;
        titleText.text = title;
        subtitleText.text = subtitle;
    }
}
