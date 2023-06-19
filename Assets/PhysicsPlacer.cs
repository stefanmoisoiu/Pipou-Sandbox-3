#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class PhysicsPlacer : EditorWindow
{
    private static bool _rotateRandomlyAroundAxis=true,_faceNormal=true;
    public enum UpDir {Up,Down,Forward,Backward,Left,Right}

    private static Enum _upDir = UpDir.Up;
    private bool scaleX=true, scaleY=true, scaleZ=true,uniformScale = false;
    private float minScaleAmount=0.9f,maxScaleAmount=1.1f;
    private static float _placeYOffset;

    private GUIStyle titleStyle, subtitleStyle;
        
        
    
    [MenuItem("Window/Physics Placer")]
    public static void ShowWindow()
    {
        GetWindow<PhysicsPlacer>("Physics Placer");
    }
    private void OnEnable()
    {
        SceneView.duringSceneGui += ShowGizmos;
        
        titleStyle = new()
        {
            fontSize = 32,
            fontStyle = FontStyle.Bold,
            margin = new RectOffset(0, 0, 10, 10),
            alignment = TextAnchor.MiddleCenter,
            normal = new GUIStyleState { textColor = Color.white }
        };
        subtitleStyle = new()
        {
            fontSize = 15,
            fontStyle = FontStyle.Bold,
            margin = new RectOffset(0, 0, 5, 5),
            alignment = TextAnchor.MiddleCenter,
            normal = new GUIStyleState { textColor = Color.white }
        };
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= ShowGizmos;
    }
    private void OnGUI()
    {
        GUILayout.Label("Physics Placer",titleStyle);
        
        GUILayout.Label("Position",subtitleStyle);
        EditorGUILayout.BeginHorizontal();
        _placeYOffset = EditorGUILayout.Slider(_placeYOffset,-5,5);
        EditorGUILayout.EndHorizontal();
        
        GUILayout.Space(20);
        
        GUILayout.Label("Rotation",subtitleStyle);
        _rotateRandomlyAroundAxis = EditorGUILayout.ToggleLeft("Rotate randomly around axis",_rotateRandomlyAroundAxis);
        EditorGUILayout.BeginHorizontal();
        _faceNormal = EditorGUILayout.ToggleLeft("Face normal",_faceNormal,GUILayout.Width(100));
        GUILayout.Label("Up Direction");
        _upDir = EditorGUILayout.EnumPopup(_upDir);
        EditorGUILayout.EndHorizontal();
        
        GUILayout.Space(20);
        
        GUILayout.Label("Scale",subtitleStyle);
        EditorGUILayout.BeginHorizontal();
        scaleX = EditorGUILayout.ToggleLeft("X",scaleX,GUILayout.Width(50));
        scaleY = EditorGUILayout.ToggleLeft("Y",scaleY,GUILayout.Width(50));
        scaleZ = EditorGUILayout.ToggleLeft("Z",scaleZ,GUILayout.Width(50));
        EditorGUILayout.EndHorizontal();
        
        GUILayout.Space(5);
        GUILayout.Label("Scale Amount");
        uniformScale = EditorGUILayout.ToggleLeft(new GUIContent(text:"Uniform Scaling (Same XYZ value)"),uniformScale);
        minScaleAmount = EditorGUILayout.Slider(new GUIContent(text:"Min"),minScaleAmount, 0f, 2f);
        maxScaleAmount = EditorGUILayout.Slider(new GUIContent(text:"Max"),maxScaleAmount, 0f, 2f);

        if (GUILayout.Button("Place"))
        {
            if (Selection.count == 0) return;
            
            List<Transform> objectTransforms = new ();
            foreach (var o in Selection.objects)
            {
                var objectTransform = ((GameObject)o).transform;
                objectTransforms.Add(objectTransform);
            }

            Undo.RecordObjects(objectTransforms.ToArray(),"Used Physics Placer");
            
            foreach (Transform objectTransform in objectTransforms)
            {
                try
                {
                    if (Physics.Raycast(objectTransform.position, Vector3.down, out RaycastHit hit, 100))
                    {
                        
                        objectTransform.position = hit.point + hit.normal * _placeYOffset;

                        if (_faceNormal)
                        {
                            switch (_upDir)
                            {
                                case UpDir.Up:
                                    objectTransform.up = hit.normal;
                                    if (_rotateRandomlyAroundAxis)
                                        objectTransform.rotation *= Quaternion.Euler(0, Random.Range(-180, 180), 0);
                                    break;
                                case UpDir.Down:
                                    objectTransform.up = -hit.normal;
                                    if (_rotateRandomlyAroundAxis)
                                        objectTransform.rotation *= Quaternion.Euler(0, Random.Range(-180, 180), 0);
                                    break;
                                case UpDir.Left:
                                    objectTransform.right = -hit.normal;
                                    if (_rotateRandomlyAroundAxis)
                                        objectTransform.rotation *= Quaternion.Euler(Random.Range(-180, 180), 0, 0);
                                    break;
                                case UpDir.Right:
                                    objectTransform.right = hit.normal;
                                    if (_rotateRandomlyAroundAxis)
                                        objectTransform.rotation *= Quaternion.Euler(Random.Range(-180, 180), 0, 0);
                                    break;
                                case UpDir.Forward:
                                    objectTransform.forward = hit.normal;
                                    if (_rotateRandomlyAroundAxis)
                                        objectTransform.rotation *= Quaternion.Euler(0, 0, Random.Range(-180, 180));
                                    break;
                                case UpDir.Backward:
                                    objectTransform.forward = -hit.normal;
                                    if (_rotateRandomlyAroundAxis)
                                        objectTransform.rotation *= Quaternion.Euler(0, 0, Random.Range(-180, 180));
                                    break;
                            }
                        }
                        if (uniformScale)
                        {
                            float scaleAmount = Random.Range(maxScaleAmount, minScaleAmount);
                            objectTransform.localScale = new Vector3(
                                scaleX
                                    ? objectTransform.localScale.x * scaleAmount
                                    : objectTransform.localScale.x,
                                scaleY
                                    ? objectTransform.localScale.y * scaleAmount
                                    : objectTransform.localScale.y,
                                scaleZ
                                    ? objectTransform.localScale.z * scaleAmount
                                    : objectTransform.localScale.z);
                        }
                        else
                        {
                            objectTransform.localScale = new Vector3(
                                scaleX
                                    ? objectTransform.localScale.x * Random.Range(maxScaleAmount, minScaleAmount)
                                    : objectTransform.localScale.x,
                                scaleY
                                    ? objectTransform.localScale.y * Random.Range(maxScaleAmount, minScaleAmount)
                                    : objectTransform.localScale.y,
                                scaleZ
                                    ? objectTransform.localScale.z * Random.Range(maxScaleAmount, minScaleAmount)
                                    : objectTransform.localScale.z);
                        }
                    }
                }
                catch
                {
                    Debug.LogError("Failed to place an object.");
                }
            }
        }
    }
    
    private static void ShowGizmos(SceneView sceneView)
    {
        Handles.BeginGUI();
        if (Selection.count > 0)
        {
            foreach (object obj in Selection.objects)
            {
                try
                {
                    Transform objectTransform = ((GameObject)obj).transform;
                    if (Physics.Raycast(objectTransform.position, Vector3.down, out RaycastHit hit, 100))
                    {
                        Debug.DrawLine(objectTransform.position, hit.point, Color.red);
                        Debug.DrawLine(hit.point, hit.point + hit.normal, Color.blue);
                        Debug.DrawLine(hit.point,hit.point + hit.normal * _placeYOffset, Color.green);
                    }
                    else
                    {
                        Debug.DrawLine(objectTransform.position,objectTransform.position + Vector3.down * 100,Color.red);  
                    }
                    
                    if (_faceNormal)
                    {
                        switch (_upDir)
                        {
                            case UpDir.Up:
                                Debug.DrawRay(objectTransform.transform.position,objectTransform.up,Color.magenta);
                                break;
                            case UpDir.Down:
                            Debug.DrawRay(objectTransform.transform.position,-objectTransform.up,Color.magenta);
                                break;
                            case UpDir.Left:
                            Debug.DrawRay(objectTransform.transform.position,-objectTransform.right,Color.magenta);
                                break;
                            case UpDir.Right:
                            Debug.DrawRay(objectTransform.transform.position,objectTransform.right,Color.magenta);
                                break;
                            case UpDir.Forward:
                            Debug.DrawRay(objectTransform.transform.position,objectTransform.forward,Color.magenta);
                                break;
                            case UpDir.Backward:
                            Debug.DrawRay(objectTransform.transform.position,-objectTransform.forward,Color.magenta);
                                break;
                        }
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }
        Handles.EndGUI();
    }
}
#endif