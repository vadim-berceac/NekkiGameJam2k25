using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Random = UnityEngine.Random;

public class CharacterCore : MonoBehaviour
{
   [field: SerializeField] public Transform PortraitCameraTransform { get; set; }
   [field: SerializeField] public Collider Collider { get; private set; }
   [field: SerializeField] public Animator Animator { get; private set; }
   [field: SerializeField] public GameObject TestEmission { get; private set; }
   [field: SerializeField] public NavMeshSettings NavMeshSettings { get; set; }
   [field: SerializeField] public Customizer Customizer { get; set; }
   
   private CharacterContainer _characterContainer;
   public Transform CharacterTransform { get; set; }
   
   private float _currentPointSpawnInterval;
   private float _timer;
   private float _normalizedSpeed;
   private Vector3 _rotationDirection;
   private const float ErrorOffset = 0.1f;

   private bool _onAction;
   
   private readonly int _moveSpeedHash = Animator.StringToHash("MoveSpeed");
   private readonly int _failureHash = Animator.StringToHash("Failure");
   private readonly int _successHash = Animator.StringToHash("Success");

   [Inject]
   private void Construct(CharacterContainer characterContainer)
   {
      CharacterTransform = transform;
      
      _rotationDirection = NavMeshSettings.Agent.steeringTarget;
      
      _characterContainer = characterContainer;
      
      _characterContainer.RegisterCharacter(this);

      _currentPointSpawnInterval = 0;
   }

   public void OnFailure()
   {
      _onAction = true;
      Animator.SetTrigger(_failureHash);
   }

   public void OnSuccess()
   {
      _onAction = true;
      Animator.SetTrigger(_successHash);
   }

   public void OnAnimationEnd()
   {
      _onAction = false;
   }
   
   private void Update()
   {
      UpdateMovement();
      
      _timer += Time.deltaTime;
      
      if (_timer < _currentPointSpawnInterval)
      {
        return;
      }
      
      _timer = 0f;
      _currentPointSpawnInterval = UpdatePointSpawnInterval();
      UpdateNewNavmeshWalkablePoint();
   }

   private void UpdateMovement()
   {
      if (_onAction)
      {
         NavMeshSettings.Agent.updatePosition = false;
         NavMeshSettings.Agent.nextPosition = CharacterTransform.position;
         return;
      }

      UpdateRotationDirection();
      UpdateVelocity();
      Rotate();
      UpdateAnimatorMoveSpeed();
      NavMeshSettings.Agent.updatePosition = true;
   }
   
   private void UpdateAnimatorMoveSpeed()
   {
      _normalizedSpeed = Mathf.Clamp01(NavMeshSettings.Agent.velocity.magnitude / NavMeshSettings.Agent.speed);

      Animator.SetFloat(_moveSpeedHash, _normalizedSpeed);
   }

   private float UpdatePointSpawnInterval()
   {
      return Random.Range(NavMeshSettings.MinPointSpawnInterval, NavMeshSettings.MaxPointSpawnInterval);
   }

   private void UpdateNewNavmeshWalkablePoint()
   {
      var randomPoint = GetRandomPointOnNavMesh();
      NavMeshSettings.Agent.SetDestination(randomPoint);
   }

   private Vector3 GetRandomPointOnNavMesh()
   {
      var randomDirection = Random.insideUnitSphere * NavMeshSettings.RandomNavMeshPointRadius;
      randomDirection += CharacterTransform.position;
      NavMesh.SamplePosition(randomDirection, out var hit, NavMeshSettings.RandomNavMeshPointRadius, NavMesh.AllAreas);
      return hit.position;
   }

   private void UpdateRotationDirection()
   {        
      var targetDirection = NavMeshSettings.Agent.steeringTarget - CharacterTransform.position;

      _rotationDirection = targetDirection.sqrMagnitude < ErrorOffset * ErrorOffset ? Vector3.zero : targetDirection.normalized;
   }
   
   private void Rotate()
   {
      if (_rotationDirection == Vector3.zero)
      {
         return;
      }
      var lookRotation = Quaternion.LookRotation(new Vector3(_rotationDirection.x, 0, _rotationDirection.z));
      CharacterTransform.rotation = Quaternion.Slerp(CharacterTransform.rotation, lookRotation, Time.deltaTime * 3);
   }

   private void UpdateVelocity()
   {
      if (!NavMeshSettings.Agent.hasPath) { return; }
      NavMeshSettings.Agent.acceleration = (NavMeshSettings.Agent.remainingDistance < NavMeshSettings.CloseEnoughMeters)
         ? NavMeshSettings.Deceleration : NavMeshSettings.Acceleration;
   }

   private void OnDestroy()
   {
      _characterContainer.UnregisterCharacter(this);
   }
}

[System.Serializable]
public struct NavMeshSettings
{
   [field: SerializeField] public NavMeshAgent Agent { get; private set; }
   [field: SerializeField] public float RandomNavMeshPointRadius { get; private set; }
   [field: SerializeField] public float MinPointSpawnInterval { get; private set; }
   [field: SerializeField] public float MaxPointSpawnInterval { get; private set; }
   [field: SerializeField] public float Acceleration { get; private set; }
   [field: SerializeField] public float Deceleration { get; private set; }
   [field: SerializeField] public float CloseEnoughMeters { get; private set; }
}
