using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [field: SerializeField] public PlayerMovementSettings PlayerMovementSettings { get; set; }
    [field: SerializeField] public Transform CameraTarget { get; set; }
    private Vector2 _moveInput;
    private Vector3 _lastPosition;
    private InputAction _moveAction;
    private float _normalizedSpeed;
    private float _smoothedSpeed; 
    private readonly int _moveSpeedHash = Animator.StringToHash("MoveSpeed");

    private void Start()
    {
        _lastPosition = transform.position;
    }
    
    private void OnEnable()
    {
        _moveAction = PlayerMovementSettings.ActionAsset.FindAction("Move");
        _moveAction.performed += OnMovePerformed;
        _moveAction.canceled += OnMoveCanceled;
    }

    private void OnDisable()
    {
        _moveAction.performed -= OnMovePerformed;
        _moveAction.canceled -= OnMoveCanceled;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _moveInput = Vector2.zero;
    }

    private void Update()
    {
        UpdateDirection();
        UpdateAnimatorMoveSpeed();
        _lastPosition = transform.position;
    }

    private void UpdateDirection()
    {
        var inputDirection = new Vector3(_moveInput.x, 0, _moveInput.y);

        if (inputDirection.sqrMagnitude <= 0.01f)
        {
            return;
        }

        var cameraYaw = CameraTarget.eulerAngles.y;
        var rotation = Quaternion.Euler(0, cameraYaw, 0);

        var worldDirection = rotation * inputDirection.normalized;

        var targetPos = transform.position + PlayerMovementSettings.Agent.speed * Time.deltaTime * worldDirection;

        if (NavMesh.SamplePosition(targetPos, out var hit, 0.5f, NavMesh.AllAreas))
        {
            PlayerMovementSettings.Agent.Move(hit.position - transform.position);

            transform.rotation = Quaternion.Euler(0, cameraYaw, 0);
        }
    }
    
    private void UpdateAnimatorMoveSpeed()
    {
        var displacement = transform.position - _lastPosition;
        var currentSpeed = displacement.magnitude / Time.deltaTime;

        var targetNormalizedSpeed = Mathf.Clamp01(currentSpeed / PlayerMovementSettings.Agent.speed);
       
        _smoothedSpeed = Mathf.Lerp(_smoothedSpeed, targetNormalizedSpeed, PlayerMovementSettings.SmoothTime);

        PlayerMovementSettings.Animator.SetFloat(_moveSpeedHash, _smoothedSpeed);
    }
}

[System.Serializable]
public struct PlayerMovementSettings
{
    [field: SerializeField] public Animator Animator { get; set; }
    [field: SerializeField] public NavMeshAgent Agent { get; private set; }
    [field: SerializeField] public InputActionAsset ActionAsset { get; set; }
    [field: SerializeField] public float SmoothTime { get; set; }
}
