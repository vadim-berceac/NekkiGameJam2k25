using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField] private GameObject interfacePrefab;
    [SerializeField] private GameObject characterContainer;
    [SerializeField] private GameObject characterSpawner;
    [SerializeField] private GameObject sceneCamera;
    public override void InstallBindings()
    {
        Container.Bind<InterfaceBase>().FromComponentInNewPrefab(interfacePrefab).AsSingle().NonLazy();
        Container.Bind<CharacterContainer>().FromComponentInNewPrefab(characterContainer).AsSingle().NonLazy();
        Container.Bind<CharacterSpawner>().FromComponentInNewPrefab(characterSpawner).AsSingle().NonLazy();
        Container.Bind<SceneCamera>().FromComponentInNewPrefab(sceneCamera).AsSingle().NonLazy();
    }
}