using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class SceneCamera : MonoBehaviour
{ 
   [field: SerializeField] public InputActionAsset ActionAsset { get; set; }
   [field: SerializeField] public SceneCameraSettings Settings { get; set; }
   [field: SerializeField] public CinemachineCamera VirtualCamera { get; set; }
   
   private Transform _cameraTransform;
   public const float Threshold = 0.01f;
   private float _targetYaw;
   private float _targetPitch;
   private Vector2 _playerLook;
   private InputAction _lookAction;
   
   private Transform _playerTransform;

   [Inject]
   private void Construct(PlayerMovement playerMovement)
   {
       _playerTransform = playerMovement.CameraTarget;
   }

   private void Awake()
   {
       _cameraTransform = transform;
       _cameraTransform.position = Settings.StartPosition;
       _cameraTransform.rotation = Settings.StartRotation;
       VirtualCamera.Follow = _playerTransform;
       VirtualCamera.LookAt = _playerTransform;
       Cursor.lockState = CursorLockMode.Locked;
       Cursor.visible = false;
   }

   private void OnEnable()
   {
       _lookAction = ActionAsset.FindAction("Look");
       _lookAction.performed += OnLookPerformed;
       _lookAction.canceled += OnLookCanceled;
   }

   private void OnDisable()
   {
       _lookAction.performed -= OnLookPerformed;
       _lookAction.canceled -= OnLookCanceled;
   }
   
   private void OnLookPerformed(InputAction.CallbackContext context)
   {
       _playerLook = context.ReadValue<Vector2>();
   }

   private void OnLookCanceled(InputAction.CallbackContext context)
   {
       _playerLook = Vector2.zero;
   }
   
   private void LateUpdate()
   {
       CameraRotation();
   }
   
   private void CameraRotation()
   {
       if (EndWindow.GameEnded) return;
       if (_playerTransform == null) return;
       if (_playerLook.sqrMagnitude >= Threshold)
       {
           _targetYaw += _playerLook.x * Settings.RotationSpeed;
           _targetPitch += _playerLook.y * Settings.RotationSpeed;
       }
      
       _targetYaw = ClampAngle( _targetYaw, float.MinValue, float.MaxValue);
       _targetPitch = ClampAngle(_targetPitch, Settings.BottomClamp, Settings.TopClamp);

       _playerTransform.transform.rotation = Quaternion.Euler(_targetPitch + Settings.CameraAngleOverride,
           _targetYaw, 0.0f);
   }
   
   private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
   {
       if (lfAngle < -360f) lfAngle += 360f;
       if (lfAngle > 360f) lfAngle -= 360f;
       return Mathf.Clamp(lfAngle, lfMin, lfMax);
   }
}

[System.Serializable]
public struct SceneCameraSettings
{
    [field: SerializeField] public Camera MainCamera { get; private set; }
    [field: SerializeField] public Vector3 StartPosition { get; private set; }
    [field: SerializeField] public Quaternion StartRotation { get; private set; }
    
    [field: Space(3)]
    [field: Header("CineMachine Settings")]
    [field: SerializeField] public float TopClamp { get; set; }
    [field: SerializeField] public float BottomClamp { get; set; }
    [field: SerializeField] public float CameraAngleOverride { get; set; }
    [field: SerializeField] public float RotationSpeed { get; set; }
}
