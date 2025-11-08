using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterContainer : MonoBehaviour
{
   private readonly HashSet<CharacterCore> _characters = new HashSet<CharacterCore>();
   private int _characterCount;
   private Action _onCountReached;
   
   public CharacterCore WantedCharacter { get; private set; }

   private void Awake()
   {
      _onCountReached += SelectWantedCharacter;
   }

   public List<CharacterCore> GetCharacters()
   {
      return _characters.ToList();
   }

   public void SetMaxCharacterCount(int count)
   {
      _characterCount = count;
   }
   
   public void RegisterCharacter(CharacterCore character)
   {
      _characters.Add(character);

      if (_characters.Count == _characterCount)
      {
         _onCountReached.Invoke();
      }
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

   private void SelectWantedCharacter()
   {
      WantedCharacter = GetCharacters()[Random.Range(0, GetCharacters().Count)];
      WantedCharacter.TestEmission.SetActive(true);
   }

   private void OnDestroy()
   {
      _onCountReached -= SelectWantedCharacter;
      ClearAllCharacters();
   }
}
