using System.Collections;
using UnityEngine;

namespace VRTraining.Gameplay.Weapons
{
    public sealed class Weapon : MonoBehaviour
    {
        [Header("Ballistics")] [SerializeField]
        private float _damage = 34f;

        [SerializeField] private float _range = 60f;
        [SerializeField] private float _secondsBetweenShots = 0.25f;
        [SerializeField] private LayerMask _hitMask = ~0;

        [Header("Setup")] [SerializeField] private Transform _muzzle;

        [SerializeField] private LineRenderer _tracer;
        [SerializeField] private float _tracerSeconds = 0.05f;

        private float _nextAllowedShotTime;
        private Coroutine _tracerRoutine;

        public Transform Muzzle => _muzzle != null ? _muzzle : transform;

        private void Awake()
        {
            if (_tracer != null)
            {
                _tracer.enabled = false;
            }
        }

        public void TryFire()
        {
            if (Time.time < _nextAllowedShotTime)
            {
                return;
            }

            _nextAllowedShotTime = Time.time + _secondsBetweenShots;
            Fire();
        }

        private void Fire()
        {
            var muzzle = Muzzle;
            var origin = muzzle.position;
            var direction = muzzle.forward;

            var endPoint = origin + direction * _range;

            if (Physics.Raycast(origin, direction, out var hit, _range, _hitMask, QueryTriggerInteraction.Ignore))
            {
                endPoint = hit.point;

                var damageable = hit.collider.GetComponentInParent<IDamageable>();
                if (damageable != null && damageable.IsAlive)
                {
                    damageable.TakeDamage(_damage, hit.point, hit.normal);
                }
            }

            ShowTracer(origin, endPoint);
        }

        private void ShowTracer(Vector3 from, Vector3 to)
        {
            if (_tracer == null)
            {
                return;
            }

            if (_tracerRoutine != null)
            {
                StopCoroutine(_tracerRoutine);
            }

            _tracerRoutine = StartCoroutine(TracerRoutine(from, to));
        }

        private IEnumerator TracerRoutine(Vector3 from, Vector3 to)
        {
            _tracer.positionCount = 2;
            _tracer.SetPosition(0, from);
            _tracer.SetPosition(1, to);
            _tracer.enabled = true;

            yield return new WaitForSeconds(_tracerSeconds);

            _tracer.enabled = false;
            _tracerRoutine = null;
        }
    }
}