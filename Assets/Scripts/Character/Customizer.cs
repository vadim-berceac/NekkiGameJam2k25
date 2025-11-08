using System.Collections.Generic;
using UnityEngine;

public class Customizer : MonoBehaviour
{
   [field: SerializeField] public CustomizerSettings Settings { get; set; }

   private readonly List<GameObject> _activeBodyParts = new List<GameObject>();

   private void Awake()
   {
       Generate();
   }

   public void Generate()
   {
       ResetBodyParts();
       
       var randomBody = Settings.Bodies[Random.Range(0, Settings.Bodies.Length)];
       var randomHead = Settings.Heads[Random.Range(0, Settings.Heads.Length)];
       var randomRHandItem = Settings.RHandItems[Random.Range(0, Settings.RHandItems.Length)];
       var randomLHandItem = Settings.LHandItems[Random.Range(0, Settings.LHandItems.Length)];

      ApplyRandomMaterial(randomBody);
      ApplyRandomMaterial(randomHead);
      ApplyRandomMaterial(randomRHandItem);
      ApplyRandomMaterial(randomLHandItem);
   }
   
   private void ApplyRandomMaterial(Renderer r)
   {
       var mats = r.materials;
       mats[0] = Settings.Materials[Random.Range(0, Settings.Materials.Length)];
       r.materials = mats;
       r.gameObject.SetActive(true);
       _activeBodyParts.Add(r.gameObject);
   }

   private void ResetBodyParts()
   {
       if (_activeBodyParts == null || _activeBodyParts.Count == 0)
       {
           return;
       }

       foreach (var part in _activeBodyParts)
       {
           part.SetActive(false);
       }
       
       _activeBodyParts.Clear();
   }
}

[System.Serializable]
public struct CustomizerSettings
{
    [field: SerializeField] public Material[] Materials { get; set; }
    [field: SerializeField] public SkinnedMeshRenderer[] Bodies { get; set; }
    [field: SerializeField] public MeshRenderer[] Heads { get; set; }
    [field: SerializeField] public MeshRenderer[] RHandItems { get; set; }
    [field: SerializeField] public MeshRenderer[] LHandItems { get; set; }
    //[field: SerializeField] public MeshRenderer[] Quivers { get; set; }
}