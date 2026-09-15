using System;
using System.Collections;
using UnityEngine;

namespace VRTraining.Gameplay.Weapons
{
    public sealed class ShootableTarget : MonoBehaviour, IDamageable
    {
        [SerializeField] private float _maxHealth = 100f;

        [Header("Death presentation")]
        [Tooltip("За столько секунд мишень заваливается после уничтожения")]
        [SerializeField]
        private float _fallSeconds = 0.45f;

        [SerializeField] private Color _deadColor = new Color(0.25f, 0.25f, 0.28f);

        private float _health;
        private Renderer[] _renderers;

        // Через событие о смерти узнает сценарий, сама мишень про него не знает
        public event Action Died;

        public bool IsAlive => _health > 0f;

        private void Awake()
        {
            _health = _maxHealth;
            _renderers = GetComponentsInChildren<Renderer>();
        }

        public void TakeDamage(float amount, Vector3 hitPoint, Vector3 hitNormal)
        {
            if (!IsAlive || amount <= 0f)
            {
                return;
            }

            _health -= amount;
            if (IsAlive)
            {
                return;
            }

            _health = 0f;
            Die();
            Died?.Invoke();
        }

        private void Die()
        {
            foreach (var targetCollider in GetComponentsInChildren<Collider>())
            {
                targetCollider.enabled = false;
            }

            foreach (var targetRenderer in _renderers)
            {
                var block = new MaterialPropertyBlock();
                targetRenderer.GetPropertyBlock(block);
                block.SetColor("_BaseColor", _deadColor);
                block.SetColor("_Color", _deadColor);
                targetRenderer.SetPropertyBlock(block);
            }

            StartCoroutine(FallOver());
        }

        private IEnumerator FallOver()
        {
            var startRotation = transform.rotation;
            var endRotation = startRotation * Quaternion.Euler(84f, 0f, 0f);
            var elapsed = 0f;

            while (elapsed < _fallSeconds)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.SmoothStep(0f, 1f, elapsed / _fallSeconds);
                transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
                yield return null;
            }

            transform.rotation = endRotation;
        }
    }
}