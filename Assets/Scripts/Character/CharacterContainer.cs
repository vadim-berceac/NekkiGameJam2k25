using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class CharacterContainer : MonoBehaviour
{
   private readonly HashSet<CharacterCore> _characters = new HashSet<CharacterCore>();
   private int _characterCount;
   private Action _onCountReached;
   
   public CharacterCore WantedCharacter { get; private set; }
   
   private CharacterPhotoURP _characterPhotoURP;
   private bool _photoCreated;

   [Inject]
   private void Construct(CharacterPhotoURP characterPhotoURP)
   {
      _characterPhotoURP = characterPhotoURP;
   }

   private void Awake()
   {
      _onCountReached += SelectWantedCharacter;
   }

   private void Update()
   {
      if (WantedCharacter != null && !_photoCreated)
      {
         _characterPhotoURP.CreatePhoto(WantedCharacter.PortraitCameraTransform);
         _photoCreated = true;
      }
   }

   public List<CharacterCore> GetCharacters()
   {
      return _characters.ToList();
   }

   public CharacterCore GetByCollider(Collider col)
   {
      return _characters.FirstOrDefault(c => c.Collider == col);
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
      _photoCreated = false;
   }

   private void OnDestroy()
   {
      _onCountReached -= SelectWantedCharacter;
      ClearAllCharacters();
   }
}
