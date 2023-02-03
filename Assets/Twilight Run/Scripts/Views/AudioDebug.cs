using System;
using UnityEngine;

namespace Avangardum.TwilightRun.Views
{
    public class AudioDebug : MonoBehaviour
    {
        public AudioSource AudioSource;
        public bool IsPlaying;

        private void Update()
        {
            IsPlaying = AudioSource.isPlaying;
        }
    }
}