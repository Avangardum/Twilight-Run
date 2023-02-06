using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Avangardum.TwilightRun.Views
{
    [RequireComponent(typeof(Button))]
    public class ButtonClickSoundPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip _clickSound;
        
        private Button _button;
        private AudioSource _audioSource;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _audioSource = GameObject.Find("UI Audio Source").GetComponent<AudioSource>();
            Assert.IsNotNull(_audioSource);
            
            _button.onClick.AddListener(PlaySound);
        }

        private void PlaySound()
        {
            _audioSource.PlayOneShot(_clickSound);
        }
    }
}