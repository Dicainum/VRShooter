using UnityEngine;

namespace VRTraining.Gameplay.Interaction
{
    [DisallowMultipleComponent]
    public sealed class ScenarioTarget : MonoBehaviour
    {
        [Tooltip("Должен совпадать с 'target' в конфиге сценария")] [SerializeField]
        private string _targetId;

        public string TargetId => _targetId;

        public void SetTargetId(string targetId)
        {
            _targetId = targetId;
        }

        private void OnEnable()
        {
            ScenarioTargetRegistry.Register(this);
        }

        private void OnDisable()
        {
            ScenarioTargetRegistry.Unregister(this);
        }
    }
}