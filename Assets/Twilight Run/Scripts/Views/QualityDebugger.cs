using TMPro;
using UnityEngine;

namespace Avangardum.TwilightRun.Views
{
    public class QualityDebugger : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _fpsText;
        [SerializeField] private TextMeshProUGUI _qualityText;
        
        private void Update()
        {
            _fpsText.text = $"FPS: {1f / Time.deltaTime:0.0}";
            _qualityText.text = $"Quality: {QualitySettings.GetQualityLevel()}";
        }
    }
}