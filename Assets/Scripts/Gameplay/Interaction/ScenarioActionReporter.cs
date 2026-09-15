using UnityEngine;
using VRTraining.Core.Events;
using VRTraining.Core.Services;
using VRTraining.Scenario.Data;
using VRTraining.Scenario.Runtime;

namespace VRTraining.Gameplay.Interaction
{
    [RequireComponent(typeof(ScenarioTarget))]
    public abstract class ScenarioActionReporter : MonoBehaviour
    {
        private ScenarioTarget _target;
        private IEventBus _bus;

        protected string TargetId => _target != null ? _target.TargetId : null;

        protected virtual void Awake()
        {
            _target = GetComponent<ScenarioTarget>();
            _bus = ServiceLocator.Get<IEventBus>();
        }

        protected void Report(ExpectedAction action)
        {
            Report(action, transform.position);
        }

        protected void Report(ExpectedAction action, Vector3 worldPosition)
        {
            if (string.IsNullOrWhiteSpace(TargetId))
            {
                return;
            }

            _bus.Publish(new PlayerActionPerformedEvent(action, TargetId, worldPosition));
        }
    }
}