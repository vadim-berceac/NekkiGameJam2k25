using UnityEngine;
using Zenject;

public class CharacterCore : MonoBehaviour
{
   [field: SerializeField] public Customizer Customizer { get; set; }
   
   private CharacterContainer _characterContainer;

   [Inject]
   private void Construct(CharacterContainer characterContainer)
   {
      _characterContainer = characterContainer;
      
      _characterContainer.RegisterCharacter(this);
   }
}
