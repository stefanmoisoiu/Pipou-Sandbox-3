using UnityEngine;

public class CanvasCameraFinder : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    public enum CamType {Main,UI}

    public CamType camType = CamType.UI;
    private void Start()
    {
        if (canvas.worldCamera == null)
        {
            if(camType == CamType.Main) canvas.worldCamera = Camera.main;
            if(camType == CamType.UI) canvas.worldCamera = GameObject.FindWithTag("UI Camera").GetComponent<Camera>();
        }
        canvas.planeDistance = 10;
    }
}
