using UnityEngine;

namespace Avangardum.TwilightRun.Main
{
    public class AutoQualitySetter : MonoBehaviour
    {
        private const bool ApplyExpensiveChanges = true;
        
        private float? _previousFrameTime;

        private void Awake()
        {
            Application.targetFrameRate = 60;
        }

        private void Update()
        {
            var currentFrameTime = Time.deltaTime;

            const float startupInactivityTime = 1f;
            if (_previousFrameTime.HasValue && Time.time > startupInactivityTime)
            {
                const float frameTimeToDecreaseQuality = 1f / 30f;
                const float frameTimeToIncreaseQuality = 1f / 55f;
                if (currentFrameTime > frameTimeToDecreaseQuality && _previousFrameTime.Value > frameTimeToDecreaseQuality)
                {
                    QualitySettings.DecreaseLevel(ApplyExpensiveChanges);
                }
                else if (currentFrameTime < frameTimeToIncreaseQuality && _previousFrameTime.Value < frameTimeToIncreaseQuality)
                {
                    QualitySettings.IncreaseLevel(ApplyExpensiveChanges);
                }
            }
            
            _previousFrameTime = currentFrameTime;
        }
    }
}