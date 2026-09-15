using System;
using UnityEngine;
using VRTraining.Core.Events;
using VRTraining.Core.Services;
using VRTraining.Scenario.Runtime;

namespace VRTraining.Gameplay.Interaction
{
    // Держит включенной подсветку только у цели активного шага
    public sealed class ScenarioHighlighter : MonoBehaviour
    {
        private IEventBus _bus;
        private TargetHighlight _current;

        private Action<StepActivatedEvent> _onStepActivated;
        private Action<ScenarioCompletedEvent> _onCompleted;

        private void Awake()
        {
            _bus = ServiceLocator.Get<IEventBus>();

            _onStepActivated = evt => Show(evt.Step.Definition.TargetId);
            _onCompleted = _ => Show(null);

            _bus.Subscribe(_onStepActivated);
            _bus.Subscribe(_onCompleted);
        }

        private void OnDestroy()
        {
            if (_bus == null)
            {
                return;
            }

            _bus.Unsubscribe(_onStepActivated);
            _bus.Unsubscribe(_onCompleted);
        }

        private void Show(string targetId)
        {
            if (_current != null)
            {
                _current.SetVisible(false);
                _current = null;
            }

            if (string.IsNullOrEmpty(targetId) || !ScenarioTargetRegistry.TryGet(targetId, out var target))
            {
                return;
            }

            _current = target.GetComponent<TargetHighlight>();
            if (_current != null)
            {
                _current.SetVisible(true);
            }
        }
    }
}
