using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Avangardum.TwilightRun.Main
{
    public class AutoQualitySetter : MonoBehaviour
    {
        private const bool ApplyExpensiveChanges = true;
        
        [SerializeField] private GameObject _loadingScreen;
        
        private List<float> _frameTimes = new();

        private void Awake()
        {
            Application.targetFrameRate = 60;
            Assert.AreEqual(1, QualitySettings.GetQualityLevel());
        }

        private void Update()
        {
            const float benchmarkStartTime = 1f;
            const float benchmarkEndTime = 2f;
            if (Time.time is >= benchmarkStartTime and <= benchmarkEndTime)
            {
                _frameTimes.Add(Time.deltaTime);
            }
            else if (Time.time > benchmarkEndTime)
            {
                const float frameTimeToDecreaseQuality = 1f / 30f;
                const float frameTimeToIncreaseQuality = 1f / 55f;
                var averageFrameTime = _frameTimes.Average();
                if (averageFrameTime > frameTimeToDecreaseQuality)
                {
                    QualitySettings.DecreaseLevel(ApplyExpensiveChanges);
                }
                else if (averageFrameTime < frameTimeToIncreaseQuality)
                {
                    QualitySettings.IncreaseLevel(ApplyExpensiveChanges);
                }
                Destroy(_loadingScreen);
                Destroy(gameObject);
            }
        }
    }
}