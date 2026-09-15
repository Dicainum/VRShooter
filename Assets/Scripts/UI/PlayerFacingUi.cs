using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace VRTraining.UI
{
    // Ленивое слежение берём готовое из XRI
    [RequireComponent(typeof(LazyFollow))]
    public sealed class PlayerFacingUi : MonoBehaviour
    {
        [Tooltip("На сколько метров перед лицом висит панель")] [SerializeField]
        private float _distance = 1.8f;

        [Tooltip("Смещение по высоте относительно линии взгляда")] [SerializeField]
        private float _height = -0.2f;

        private LazyFollow _follow;

        private void Awake()
        {
            _follow = GetComponent<LazyFollow>();
            _follow.targetOffset = new Vector3(0f, _height, _distance);
            _follow.rotationFollowMode = LazyFollow.RotationFollowMode.LookAtWithWorldUp;
        }

        private void Start()
        {
            var playerCamera = Camera.main;
            _follow.target = playerCamera != null ? playerCamera.transform : null;

            ShowInFrontOfPlayer();
        }

        public void ShowInFrontOfPlayer()
        {
            if (!isActiveAndEnabled || _follow.target == null)
            {
                return;
            }

            _follow.enabled = false;
            _follow.enabled = true;
        }
    }
}