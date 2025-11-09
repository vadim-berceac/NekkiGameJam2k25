using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerMovement : MonoBehaviour
{
    [field: SerializeField] public PlayerMovementSettings PlayerMovementSettings { get; set; }
    [field: SerializeField] public Transform CameraTarget { get; set; }
    private Vector2 _moveInput;
    private Vector3 _lastPosition;
    private InputAction _moveAction;
    private InputAction _interactAction;
    private float _normalizedSpeed;
    private float _smoothedSpeed; 
    private bool _onAction;
    private const float CheckSphereRadius = 2f;
    private readonly int _moveSpeedHash = Animator.StringToHash("MoveSpeed");
    private readonly int _signHash = Animator.StringToHash("Sign");
    
    private CharacterContainer _characterContainer;

    public static Action OnWrongChose;
    public static Action OnRightChose;

    [Inject]
    private void Construct(CharacterContainer characterContainer)
    {
        _characterContainer = characterContainer;
    }

    private void Start()
    {
        _lastPosition = transform.position;
    }
    
    public void OnAnimationEnd()
    {
        _onAction = false;
    }
    
    private void OnEnable()
    {
        _moveAction = PlayerMovementSettings.ActionAsset.FindAction("Move");
        _interactAction = PlayerMovementSettings.ActionAsset.FindAction("Interact");
        _moveAction.performed += OnMovePerformed;
        _moveAction.canceled += OnMoveCanceled;
        _interactAction.started += OnInteract;
    }

    private void OnDisable()
    {
        _moveAction.performed -= OnMovePerformed;
        _moveAction.canceled -= OnMoveCanceled;
        _interactAction.performed -= OnInteract;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _moveInput = Vector2.zero;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (_onAction)
        {
            return;
        }
        _onAction = true;

        PlayerMovementSettings.Animator.SetTrigger(_signHash);

        CheckSphere();
    }

    private void CheckSphere()
    {
        var sphereCenter = transform.position + transform.forward * 1f;
        
        var hits = Physics.OverlapSphere(sphereCenter, CheckSphereRadius);

        Collider nearest = null;
        var minDist = 10f;
       
        var characterLayer = LayerMask.NameToLayer("Character");

        foreach (var hit in hits)
        {
            if (hit.gameObject.layer != characterLayer || hit.gameObject == gameObject)
            {
                continue;
            }
            
            var dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist >= minDist)
            {
                continue;
            }
            minDist = dist;
            nearest = hit;
        }

        if (nearest == null)
        {
           return;
        }
        
        var core = _characterContainer.GetByCollider(nearest);

        if (core == _characterContainer.WantedCharacter)
        {
            core.OnSuccess();
            OnRightChose?.Invoke();
            return;
        }
        core.OnFailure();
        OnWrongChose?.Invoke();
    }

    private void Update()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        if (_onAction)
        {
            return;
        }
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
