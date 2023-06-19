using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ScalingTool : MonoBehaviour
{
    [SerializeField] private float[] scaleSizes = { 0.1f, 0.25f, 0.5f, 0.75f, 1f, 2f, 3f, 4f, 5f };
    [SerializeField] private float sizeLerpSpeed = 1.5f;
    [SerializeField] private AnimationCurve sizeLerpCurve;
    private int currentSizeIndex;
    [SerializeField] private TMP_Text displayText;
    private Coroutine lerpSizeCoroutine;

    private void Start()
    {
        currentSizeIndex = scaleSizes.Length / 2;
    }

    public void AddSize(int index)
    {
        currentSizeIndex += index;
        currentSizeIndex = Mathf.Clamp(currentSizeIndex, 0, scaleSizes.Length - 1);
        
        displayText.text = scaleSizes[currentSizeIndex].ToString();
        
        if(lerpSizeCoroutine != null) StopCoroutine(lerpSizeCoroutine);
        lerpSizeCoroutine = StartCoroutine(LerpSize(scaleSizes[currentSizeIndex]));
    }
    private IEnumerator LerpSize(float to)
    {
        float startSize = transform.root.localScale.x;
        float advancement = 0;
        while (advancement < 1)
        {
            advancement += Time.deltaTime / sizeLerpSpeed;
            float currentScale = Mathf.Lerp(startSize,to,sizeLerpCurve.Evaluate(advancement));
            transform.root.localScale = new Vector3(currentScale, currentScale, currentScale);
            yield return null;
        }
    }
}
