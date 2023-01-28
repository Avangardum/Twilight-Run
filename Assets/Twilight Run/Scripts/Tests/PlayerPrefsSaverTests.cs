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
    }
}