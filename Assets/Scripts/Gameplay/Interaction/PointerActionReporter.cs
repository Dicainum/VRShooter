using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using VRTraining.Scenario.Data;

namespace VRTraining.Gameplay.Interaction
{
    public sealed class PointerActionReporter : ScenarioActionReporter
    {
        private XRBaseInteractable _interactable;

        protected override void Awake()
        {
            base.Awake();

            _interactable = GetComponent<XRBaseInteractable>();
            if (_interactable != null)
            {
                _interactable.selectEntered.AddListener(OnSelectEntered);
            }
        }

        private void OnDestroy()
        {
            if (_interactable != null)
            {
                _interactable.selectEntered.RemoveListener(OnSelectEntered);
            }
        }

        public void ReportPoint()
        {
            Report(ExpectedAction.Point);
        }

        private void OnSelectEntered(SelectEnterEventArgs args)
        {
            ReportPoint();
        }
    }
}
