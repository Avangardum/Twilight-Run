using UnityEngine;

namespace Avangardum.TwilightRun.Views
{
    [RequireComponent(typeof(RagdollControl))]
    public class RagdollControlDebug : MonoBehaviour
    {
        [SerializeField] private bool _isRagdoll;
        [SerializeField] private bool _isGravityInversed;
        
        private RagdollControl _ragdollControl;
        
        private void Awake()
        {
            _ragdollControl = GetComponent<RagdollControl>();
        }
        
        private void Update()
        {
            _ragdollControl.IsRagdoll = _isRagdoll;
            _ragdollControl.IsGravityInversed = _isGravityInversed;
        }
    }
}