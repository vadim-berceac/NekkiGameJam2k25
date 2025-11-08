using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class CharacterSpawner : MonoBehaviour
{
    [SerializeField] private CharacterCore characterPrefab;
    [SerializeField] private int spawnCount = 5;
    [SerializeField] private float spawnRadius = 20f;

    private DiContainer _container;
    private CharacterContainer _characterContainer;

    [Inject]
    private void Construct(DiContainer container, CharacterContainer characterContainer)
    {
        _container = container;
        _characterContainer = characterContainer;
    }

    private void Start()
    {
        SpawnCharacters();
    }
    
    public void SpawnCharacters()
    {
        var charList = _characterContainer.GetCharacters();
        if (charList.Count != 0)
        {
            _characterContainer.ClearAllCharacters();
        }
        
        for (var i = 0; i < spawnCount; i++)
        {
            var spawnPos = GetRandomPointOnNavMesh(transform.position, spawnRadius);
            if (spawnPos.HasValue)
            {
                _container.InstantiatePrefabForComponent<CharacterCore>(
                    characterPrefab,
                    spawnPos.Value,
                    Quaternion.identity,
                    transform
                );
            }
        }
    }

    private static Vector3? GetRandomPointOnNavMesh(Vector3 center, float radius)
    {
        for (var i = 0; i < 10; i++) 
        {
            var randomPos = center + Random.insideUnitSphere * radius;
            if (NavMesh.SamplePosition(randomPos, out var hit, radius, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        return null;
    }
}