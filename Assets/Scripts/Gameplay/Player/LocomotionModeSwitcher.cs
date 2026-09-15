using UnityEngine;

namespace VRTraining.Gameplay.Player
{
    public sealed class LocomotionModeSwitcher : MonoBehaviour
    {
        public enum LocomotionMode
        {
            Continuous,
            Teleport
        }

        [SerializeField] private LocomotionMode _mode = LocomotionMode.Continuous;

        [SerializeField] private MonoBehaviour[] _continuousProviders;

        [SerializeField] private GameObject[] _teleportObjects;

        public LocomotionMode Mode => _mode;

        private void Start()
        {
            Apply();
        }

        public void SetMode(LocomotionMode mode)
        {
            _mode = mode;
            Apply();
        }

        public void ToggleMode()
        {
            SetMode(_mode == LocomotionMode.Continuous ? LocomotionMode.Teleport : LocomotionMode.Continuous);
        }

        private void Apply()
        {
            var continuous = _mode == LocomotionMode.Continuous;

            if (_continuousProviders != null)
            {
                foreach (var provider in _continuousProviders)
                {
                    if (provider != null)
                    {
                        provider.enabled = continuous;
                    }
                }
            }

            if (_teleportObjects == null)
            {
                return;
            }

            foreach (var teleportObject in _teleportObjects)
            {
                if (teleportObject != null)
                {
                    teleportObject.SetActive(!continuous);
                }
            }
        }
    }
}