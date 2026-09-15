using UnityEngine;
using VRTraining.Gameplay.Weapons;
using VRTraining.Scenario.Data;

namespace VRTraining.Gameplay.Interaction
{
    // Отдельный компонент, чтобы мишень оставалась просто мишенью и ничего не знала про сценарий
    [RequireComponent(typeof(ShootableTarget))]
    public sealed class ShootActionReporter : ScenarioActionReporter
    {
        private ShootableTarget _target;

        protected override void Awake()
        {
            base.Awake();

            _target = GetComponent<ShootableTarget>();
            _target.Died += OnDied;
        }

        private void OnDestroy()
        {
            if (_target != null)
            {
                _target.Died -= OnDied;
            }
        }

        private void OnDied()
        {
            Report(ExpectedAction.Shoot);
        }
    }
}
