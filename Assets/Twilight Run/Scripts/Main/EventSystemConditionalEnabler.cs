using UnityEngine;
using UnityEngine.EventSystems;

namespace Avangardum.TwilightRun.Main
{
    [RequireComponent(typeof(EventSystem))]
    public class EventSystemConditionalEnabler : MonoBehaviour
    {
        private void Awake()
        {
            var eventSystem = GetComponent<EventSystem>();
            var eventSystemsCount = FindObjectsOfType<EventSystem>().Length;
            if (eventSystemsCount == 1) eventSystem.enabled = true;
        }
    }
}