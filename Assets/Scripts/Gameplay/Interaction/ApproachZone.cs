using UnityEngine;
using VRTraining.Gameplay.Player;
using VRTraining.Scenario.Data;

namespace VRTraining.Gameplay.Interaction
{
    [RequireComponent(typeof(Collider))]
    public sealed class ApproachZone : ScenarioActionReporter
    {
        protected override void Awake()
        {
            base.Awake();

            var zoneCollider = GetComponent<Collider>();
            //для уверенности, что коллайдер триггер
            if (!zoneCollider.isTrigger)
            {
                zoneCollider.isTrigger = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<PlayerMarker>() == null)
            {
                return;
            }

            Report(ExpectedAction.Approach, other.transform.position);
        }
    }
}
