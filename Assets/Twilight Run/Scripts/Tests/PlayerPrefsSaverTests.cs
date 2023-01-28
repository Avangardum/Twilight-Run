using Avangardum.TwilightRun.Savers;
using NUnit.Framework;
using Zenject;

namespace Avangardum.TwilightRun.Tests
{
    public class PlayerPrefsSaverTests : ZenjectUnitTestFixture
    {
        private PlayerPrefsSaver _saver;
        
        [Inject]
        private void Inject(PlayerPrefsSaver saver)
        {
            _saver = saver;
        }
        
        [SetUp]
        public void CSetup()
        {
            Container.Bind<PlayerPrefsSaver>().AsSingle();
            Container.Inject(this);
            _saver.Clear();
        }
        
        [TearDown]
        public void CTeardown()
        {
            _saver.Clear();
        }
        
        [Test]
        public void SavesAndLoadsHighScore([Values(1, 2, 3, 5, 8, 13, 21, 34)] int highScore)
        {
            _saver.HighScore = highScore;
            Assert.That(_saver.HighScore, Is.EqualTo(highScore));
        }
        
        [Test]
        public void ChangingHighScoreInvokesEvent()
        {
            var invoked = false;
            _saver.HighScoreChanged += (_, _) => invoked = true;
            _saver.HighScore = 1;
            Assert.That(invoked, Is.True);
        }
    }
}