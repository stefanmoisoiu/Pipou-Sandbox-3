using TMPro;
using UnityEngine;

public class CurrencyCounter : MonoBehaviour
{
    [SerializeField] private AnimationClip animToPlay;
    [SerializeField] private Animator animator;
    [SerializeField] private TMP_Text currencyAmountText;
    [SerializeField] private string currencyName = "portraits";
    
    
    private void Start()
    {
        CloudCurrency.onCurrencyValueUpdated += DisplayCurrency;
    }

    private void DisplayCurrency(int value)
    {
        currencyAmountText.text = $"{value}\n{currencyName}";
        animator.Play(animToPlay.name);
    }
}
