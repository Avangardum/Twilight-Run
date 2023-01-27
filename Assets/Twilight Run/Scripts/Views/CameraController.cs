using UnityEngine;

namespace Avangardum.TwilightRun.Views
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        
        private Vector3 _offset;
        private float _yPosition;
        
        private void Awake()
        {
            _offset = transform.position - _target.position;
            _yPosition = transform.position.y;
        }
        
        private void LateUpdate()
        {
            var position = _target.position + _offset;
            position.y = _yPosition;
            transform.position = position;
        }
    }
}