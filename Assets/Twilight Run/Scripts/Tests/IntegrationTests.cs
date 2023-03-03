using System.Collections;
using System.Linq;
using Avangardum.TwilightRun.Models;
using Avangardum.TwilightRun.Savers;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;
using UAssert = UnityEngine.Assertions.Assert;

namespace Avangardum.TwilightRun.Tests
{
    [Explicit]
    public class IntegrationTests : SceneTestFixture
    {
        private GameObject _whiteCharacter;
        private GameObject _blackCharacter;
        private IGameModel _gameModel;
        private PlayerPrefsSaver _saver;

        [Inject]
        public void Inject(IGameModel gameModel, PlayerPrefsSaver saver)
        {
            _gameModel = gameModel;
            _saver = saver;
        }
        
        [UnitySetUp]
        public IEnumerator CSetup()
        {
            Time.timeScale = 1;
            yield return LoadScene("Game");
            _saver.AreAdsRemoved = true; // Remove ads, because ads throw exceptions during tests
            _whiteCharacter = GameObject.Find("White Character");
            UAssert.IsNotNull(_whiteCharacter);
            _blackCharacter = GameObject.Find("Black Character");
            UAssert.IsNotNull(_blackCharacter);
            _gameModel.Restart();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator CTeardown()
        {
            _saver.Clear();
            Object.FindObjectsOfType<GameObject>().ToList().ForEach(Object.Destroy);
            yield break;
        }

        [UnityTest]
        public IEnumerator CharactersMoveForward()
        {
            Assert.That(_whiteCharacter.transform.position.z, Is.EqualTo(_blackCharacter.transform.position.z));
            var whiteCharacterZPos = _whiteCharacter.transform.position.z;
            var blackCharacterZPos = _blackCharacter.transform.position.z;
            yield return null;
            Assert.That(_whiteCharacter.transform.position.z, Is.EqualTo(_blackCharacter.transform.position.z));
            Assert.That(_whiteCharacter.transform.position.z, Is.GreaterThan(whiteCharacterZPos));
            Assert.That(_blackCharacter.transform.position.z, Is.GreaterThan(blackCharacterZPos));
        }

        [UnityTest]
        public IEnumerator CharactersSwap()
        {
            var whiteCharacterYPosition = _whiteCharacter.transform.position.y;
            var blackCharacterYPosition = _blackCharacter.transform.position.y;
            _gameModel.Swap();
            Time.timeScale = 5;
            yield return new WaitForSeconds(3);
            Time.timeScale = 1;
            Assert.That(_whiteCharacter.transform.position.y, Is.EqualTo(blackCharacterYPosition));
            Assert.That(_blackCharacter.transform.position.y, Is.EqualTo(whiteCharacterYPosition));
        }

        [Test]
        public void ObstaclesCreatedOnStartup()
        {
            var obstacles = Object.FindObjectsOfType<GameObject>()
                .Where(go => go.name.StartsWith("Obstacle"));
            Assert.That(obstacles, Is.Not.Empty);
        }

        [UnityTest]
        public IEnumerator HighScoreIsUpdated()
        {
            _saver.HighScore = 0;
            Time.timeScale = 5;
            yield return new WaitForSeconds(3);
            Time.timeScale = 1;
            Assert.That(_saver.HighScore, Is.GreaterThan(0));
        }
    }
}