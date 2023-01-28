using Avangardum.TwilightRun.Models;
using Avangardum.TwilightRun.Presenters;
using Avangardum.TwilightRun.Savers;
using Avangardum.TwilightRun.Views;
using UnityEngine;
using Zenject;

namespace Avangardum.TwilightRun.Main
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private GameView _gameView;
        [SerializeField] private GameConfig _gameConfig;
        
        public override void InstallBindings()
        {
            Container.Bind<IGameModel>().To<GameModel>().AsSingle();
            Container.Bind<IGameView>().FromInstance(_gameView).AsSingle();
            Container.Bind<GamePresenter>().AsSingle().NonLazy();
            Container.Bind<IGameConfig>().FromInstance(_gameConfig).AsSingle();
            Container.Bind<ISaver>().To<PlayerPrefsSaver>().AsSingle();
        }
    }
}