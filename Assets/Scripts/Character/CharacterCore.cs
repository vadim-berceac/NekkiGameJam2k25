using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class CharacterCore : MonoBehaviour
{
   [field: SerializeField] public NavMeshSettings NavMeshSettings { get; set; }
   [field: SerializeField] public Customizer Customizer { get; set; }
   
   private CharacterContainer _characterContainer;
   public Transform CharacterTransform { get; set; }
   
   
   private float _currentPointSpawnInterval;
   private float _timer;
   private Vector3 _rotationDirection;
   private const float ErrorOffset = 0.1f;

   [Inject]
   private void Construct(CharacterContainer characterContainer)
   {
      CharacterTransform = transform;
      
      _rotationDirection = NavMeshSettings.Agent.steeringTarget;
      
      _characterContainer = characterContainer;
      
      _characterContainer.RegisterCharacter(this);

      _currentPointSpawnInterval = UpdatePointSpawnInterval();
   }
   
   private void Update()
   {
      UpdateRotationDirection();
      UpdateVelocity();
      Rotate();
      
      _timer += Time.deltaTime;
      
      if (_timer < _currentPointSpawnInterval)
      {
        return;
      }
      
      _timer = 0f;
      _currentPointSpawnInterval = UpdatePointSpawnInterval();
      UpdateNewNavmeshWalkablePoint();
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
      var lookRotation = Quaternion.LookRotation(new Vector3(_rotationDirection.x, 0, _rotationDirection.z));
      CharacterTransform.rotation = Quaternion.Slerp(CharacterTransform.rotation, lookRotation, Time.deltaTime * 3);
   }

   private void UpdateVelocity()
   {
      if (!NavMeshSettings.Agent.hasPath) { return; }
      NavMeshSettings.Agent.acceleration = (NavMeshSettings.Agent.remainingDistance < NavMeshSettings.CloseEnoughMeters)
         ? NavMeshSettings.Deceleration : NavMeshSettings.Acceleration;
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
