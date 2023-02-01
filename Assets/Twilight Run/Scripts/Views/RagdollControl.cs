using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Avangardum.TwilightRun.Views
{
    [RequireComponent(typeof(Animator))]
    public class RagdollControl : MonoBehaviour
    {
        private Animator _animator;
        private List<Rigidbody> _rigidbodies;
        private List<PersonalGravity> _personalGravities;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _rigidbodies = GetComponentsInChildren<Rigidbody>().ToList();
            _personalGravities = GetComponentsInChildren<PersonalGravity>().ToList();
        }

        public bool IsRagdoll
        {
            set
            {
                _animator.enabled = !value;
                foreach (var rb in _rigidbodies)
                {
                    rb.isKinematic = !value;
                }
            }
        }

        public bool IsGravityInversed
        {
            set
            {
                foreach (var pg in _personalGravities)
                {
                    pg.IsGravityInversed = value;
                }
            }
        }
        
        public Vector3 Velocity
        {
            set
            {
                foreach (var rb in _rigidbodies)
                {
                    rb.velocity = value;
                }
            }
        }
    }
}