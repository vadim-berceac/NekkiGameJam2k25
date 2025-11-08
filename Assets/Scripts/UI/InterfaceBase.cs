using UnityEngine;
using Zenject;

public class InterfaceBase : MonoBehaviour
{
    private CharacterContainer _characterContainer;

    [Inject]
    private void Construct(CharacterContainer characterContainer)
    {
        _characterContainer = characterContainer;
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
}
