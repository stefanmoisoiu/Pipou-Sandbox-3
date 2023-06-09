using System;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStreamer : MonoBehaviour
{
    [SerializeField] private SceneToLoad[] scenesToLoad;

    private void Update()
    {
        for (int i = 0; i < scenesToLoad.Length; i++)
        {
            ManageScene(ref scenesToLoad[i]);
        }
    }

    private void ManageScene(ref SceneToLoad sceneToLoad)
    {
        if (NetworkManager.Singleton.LocalClient == null ||
            NetworkManager.Singleton.LocalClient.PlayerObject == null) return;
        Vector3 playerPos = NetworkManager.Singleton.LocalClient.PlayerObject.transform.position;
        for (int i = 0; i < sceneToLoad.scenePoints.Length; i++)
        {
            if (Vector3.Distance(sceneToLoad.scenePoints[i].pointPos.position, playerPos) <
                sceneToLoad.scenePoints[i].size)
            {
                if (sceneToLoad.loaded) return;
                Debug.Log($"Loading Scene {sceneToLoad.sceneName}");
                sceneToLoad.loaded = true;
                SceneManager.LoadSceneAsync(sceneToLoad.sceneName, LoadSceneMode.Additive);
                return;
            }
        }

        if (sceneToLoad.loaded)
        {
            Debug.Log($"Unloading Scene {sceneToLoad.sceneName}");
            sceneToLoad.loaded = false;
            SceneManager.UnloadSceneAsync(sceneToLoad.sceneName);
        }
    }

    private void OnDrawGizmos()
    {
        foreach (SceneToLoad sceneToLoad in scenesToLoad)
        {
            if(sceneToLoad == null || !sceneToLoad.showGizmos) continue;
            foreach (ScenePoint scenePoint in sceneToLoad.scenePoints)
            {
                if(scenePoint.pointPos == null) continue;
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(scenePoint.pointPos.position,0.5f);
                Gizmos.color = sceneToLoad.gizmoColor;
                if(sceneToLoad.wireOutline) Gizmos.DrawWireSphere(scenePoint.pointPos.position,scenePoint.size);
                else Gizmos.DrawSphere(scenePoint.pointPos.position,scenePoint.size);
            }
        }
    }

    [Serializable]
    public class SceneToLoad
    {
        public string sceneName;
        public ScenePoint[] scenePoints;
        [ReadOnly]public bool loaded;
        public bool showGizmos = true,wireOutline = true;
        public Color gizmoColor = Color.red;
    }
    [Serializable]
    public struct ScenePoint
    {
        public Transform pointPos;
        public float size;
    }
}
