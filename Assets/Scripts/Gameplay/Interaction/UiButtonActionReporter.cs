using UnityEngine;
using UnityEngine.UI;
using VRTraining.Scenario.Data;

namespace VRTraining.Gameplay.Interaction
{
    [RequireComponent(typeof(Button))]
    public sealed class UiButtonActionReporter : ScenarioActionReporter
    {
        private Button _button;

        protected override void Awake()
        {
            base.Awake();

            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClicked);
        }

        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(OnClicked);
            }
        }

        private void OnClicked()
        {
            Report(ExpectedAction.PressUi);
        }
    }
}
