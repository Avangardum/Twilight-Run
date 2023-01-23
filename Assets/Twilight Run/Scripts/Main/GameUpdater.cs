using Avangardum.TwilightRun.Models;
using UnityEngine;
using Zenject;

namespace Avangardum.TwilightRun.Main
{
    public class GameUpdater : MonoBehaviour
    {
        private IGameModel _gameModel;
        
        [Inject]
        public void Inject(IGameModel gameModel)
        {
            _gameModel = gameModel;
        }

        private void Update()
        {
            _gameModel.Update(Time.deltaTime);
        }
    }
}