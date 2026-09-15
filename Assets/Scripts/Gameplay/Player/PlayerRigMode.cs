using UnityEngine;

namespace VRTraining.Gameplay.Player
{
    public sealed class PlayerRigMode : MonoBehaviour
    {
        [SerializeField] private GameObject[] _vrOnly;

        [SerializeField] private GameObject[] _desktopOnly;

        [SerializeField] private bool _forceDesktop;

        public bool IsVrActive { get; private set; }

        private void Awake()
        {
            IsVrActive = !_forceDesktop && XrRuntime.IsRunning();

            SetActive(_vrOnly, IsVrActive);
            SetActive(_desktopOnly, !IsVrActive);

            Debug.Log(IsVrActive
                ? "XR loader detected"
                : "No XR loaded");
        }

        private static void SetActive(GameObject[] objects, bool active)
        {
            if (objects == null)
            {
                return;
            }

            foreach (var target in objects)
            {
                if (target != null)
                {
                    target.SetActive(active);
                }
            }
        }
    }
}