using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

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
   
   public void ClearAllCharacters()
   {
      foreach (var character in _characters.ToList())
      {
         if (character != null)
         {
            Destroy(character.gameObject);
         }
      }
      _characters.Clear();
   }

   public void UnregisterCharacter(CharacterCore character)
   {
      if (!_characters.Contains(character))
      {
         return;
      }
      _characters.Remove(character);
   }

   private void OnDestroy()
   {
      ClearAllCharacters();
   }
}
