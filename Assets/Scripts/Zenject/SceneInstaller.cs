using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField] private GameObject interfacePrefab;
    [SerializeField] private GameObject characterContainer;
    public override void InstallBindings()
    {
        Container.Bind<InterfaceBase>().FromComponentInNewPrefab(interfacePrefab).AsSingle().NonLazy();
        Container.Bind<CharacterContainer>().FromComponentInNewPrefab(characterContainer).AsSingle().NonLazy();
    }
}