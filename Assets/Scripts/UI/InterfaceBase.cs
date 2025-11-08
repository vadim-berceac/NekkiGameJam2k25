using UnityEngine;
using Zenject;

public class InterfaceBase : MonoBehaviour
{
    private CharacterContainer _characterContainer;
    private CharacterSpawner _characterSpawner;

    [Inject]
    private void Construct(CharacterContainer characterContainer, CharacterSpawner characterSpawner)
    {
        _characterContainer = characterContainer;
        _characterSpawner = characterSpawner;
    }
    
    
    public void Generate()
    {
        var chars = _characterContainer.GetCharacters();

        if (chars.Count == 0)
        {
            return;
        }

        foreach (var c in chars)
        {
            c.Customizer.Generate();
        }
    }

    public void Spawn()
    {
        _characterSpawner.SpawnCharacters();
    }
}
