using UnityEngine;

public class SceneCamera : MonoBehaviour
{
   [field: SerializeField] public SceneCameraSettings Settings { get; set; }
   
   private Transform _cameraTransform;

   private void Awake()
   {
       _cameraTransform = transform;
       _cameraTransform.position = Settings.StartPosition;
       _cameraTransform.rotation = Settings.StartRotation;
   }
}

[System.Serializable]
public struct SceneCameraSettings
{
    [field: SerializeField] public Camera MainCamera { get; private set; }
    [field: SerializeField] public Vector3 StartPosition { get; private set; }
    [field: SerializeField] public Quaternion StartRotation { get; private set; }
}
