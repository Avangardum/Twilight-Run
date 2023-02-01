using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Avangardum.TwilightRun.Views
{
    [RequireComponent(typeof(Rigidbody))]
    public class PersonalGravity : MonoBehaviour
    {
        private const float DefaultGravity = -9.81f;

        public bool IsGravityInversed { get; set; }
        
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            var gravity = IsGravityInversed ? -DefaultGravity : DefaultGravity;
            _rigidbody.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
        }
    }
}