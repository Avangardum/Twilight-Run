using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;
using UAssert = UnityEngine.Assertions.Assert;

namespace Avangardum.TwilightRun.Tests
{
    public class IntegrationTests : SceneTestFixture
    {
        private GameObject _whiteCharacter;
        private GameObject _blackCharacter;
        
        [UnitySetUp]
        public IEnumerator Setup()
        {
            yield return LoadScene("Game");
            _whiteCharacter = GameObject.Find("White Character");
            UAssert.IsNotNull(_whiteCharacter);
            _blackCharacter = GameObject.Find("Black Character");
            UAssert.IsNotNull(_blackCharacter);
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
    }
}