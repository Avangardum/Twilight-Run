using System;
using TMPro;
using UnityEngine;

namespace Avangardum.TwilightRun.Main
{
    public class UiLogger : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _logText;
        
        private void Awake()
        {
            Application.logMessageReceived += OnLogMessageReceived;
        }
        
        private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            var color = type switch
            {
                LogType.Error => "red",
                LogType.Assert => "red",
                LogType.Exception => "red",
                LogType.Warning => "yellow",
                LogType.Log => "white",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
            _logText.text += $"<color=\"{color}\">{condition}</color>\n";
        }
    }
}