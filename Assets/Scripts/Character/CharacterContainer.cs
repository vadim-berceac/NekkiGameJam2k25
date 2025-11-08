using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterContainer : MonoBehaviour
{
   private readonly HashSet<CharacterCore> _characters = new HashSet<CharacterCore>();

   public List<CharacterCore> GetCharacters()
   {
      return _characters.ToList();
   }
   
   public void RegisterCharacter(CharacterCore character)
   {
      _characters.Add(character);
   }

   public void UnregisterCharacter(CharacterCore character)
   {
      if (!_characters.Contains(character))
      {
         return;
      }
      _characters.Remove(character);
   }
}
