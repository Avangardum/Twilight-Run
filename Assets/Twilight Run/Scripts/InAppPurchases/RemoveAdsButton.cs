using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Purchasing;
using Zenject;

namespace Avangardum.TwilightRun.InAppPurchases
{
    public class RemoveAdsButton : MonoBehaviour
    {
        private ISaver _saver;
        
        [Inject]
        public void Inject(ISaver saver)
        {
            _saver = saver;
        }
        
        [UsedImplicitly]
        public void OnPurchaseComplete(Product product)
        {
            _saver.AreAdsRemoved = true;
            // Destroying without delay causes purchasing service error
            Destroy(gameObject, 0.1f);
        }
        
        private void Awake()
        {
            if (_saver.AreAdsRemoved)
            {
                Destroy(gameObject);
            }
        }
    }
}