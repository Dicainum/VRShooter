using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using VRTraining.Scenario.Data;

namespace VRTraining.Gameplay.Interaction
{
    public sealed class GrabActionReporter : ScenarioActionReporter
    {
        private XRBaseInteractable _interactable;

        protected override void Awake()
        {
            base.Awake();

            _interactable = GetComponent<XRBaseInteractable>();
            if (_interactable == null)
            {
                return;
            }

            _interactable.selectEntered.AddListener(OnSelectEntered);
            _interactable.selectExited.AddListener(OnSelectExited);
        }

        private void OnDestroy()
        {
            if (_interactable == null)
            {
                return;
            }

            _interactable.selectEntered.RemoveListener(OnSelectEntered);
            _interactable.selectExited.RemoveListener(OnSelectExited);
        }

        public void ReportGrab()
        {
            Report(ExpectedAction.Grab);
        }

        public void ReportDrop()
        {
            Report(ExpectedAction.Drop);
        }

        private void OnSelectEntered(SelectEnterEventArgs args)
        {
            ReportGrab();
        }

        private void OnSelectExited(SelectExitEventArgs args)
        {
            ReportDrop();
        }
    }
}
