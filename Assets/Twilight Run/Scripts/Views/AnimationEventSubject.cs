using System;
using UnityEngine;

namespace Avangardum.TwilightRun.Views
{
    public class AnimationEventSubject : MonoBehaviour
    {
        public event EventHandler Stepped;
        
        public void InvokeStepped()
        {
            Stepped?.Invoke(this, EventArgs.Empty);
        }
    }
}