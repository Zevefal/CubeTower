using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private TowerController _towerController;
    [SerializeField] private ScrollManager _scrollManager;
    [SerializeField] private HoleArea _holeArea;
    [SerializeField] private Canvas _canvas;

    public override void InstallBindings()
    {
        Container.Bind<TowerController>().FromInstance(_towerController).AsSingle();
        Container.Bind<ScrollManager>().FromInstance(_scrollManager).AsSingle();
        Container.Bind<HoleArea>().FromInstance(_holeArea).AsSingle();
        Container.Bind<Canvas>().FromInstance(_canvas).AsSingle();

        Container.Bind<SaveLoadService>().AsSingle();
    }
}