using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCameraFinder : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    private void Start()
    {
        if (canvas.worldCamera == null) canvas.worldCamera = GameObject.FindWithTag("UI Camera").GetComponent<Camera>();
        canvas.planeDistance = 10;
    }
}
