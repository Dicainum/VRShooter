using UnityEngine;

namespace VRTraining.Gameplay.Weapons
{
    public interface IDamageable
    {
        bool IsAlive { get; }

        void TakeDamage(float amount, Vector3 hitPoint, Vector3 hitNormal);
    }
}
