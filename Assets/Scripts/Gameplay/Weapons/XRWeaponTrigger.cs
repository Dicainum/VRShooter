using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace VRTraining.Gameplay.Weapons
{
    [RequireComponent(typeof(Weapon))]
    public sealed class XRWeaponTrigger : MonoBehaviour
    {
        [Tooltip("Основной курок левой руки")] [SerializeField]
        private InputActionReference _fireLeft;

        [Tooltip("Основной курок правой руки")] [SerializeField]
        private InputActionReference _fireRight;

        private Weapon _weapon;
        private XRBaseInteractable _interactable;

        private void Awake()
        {
            _weapon = GetComponent<Weapon>();
            _interactable = GetComponent<XRBaseInteractable>();
        }

        private void Update()
        {
            if (_interactable == null || !_interactable.isSelected)
            {
                return;
            }

            var fire = FireActionOfTheHandHolding();
            if (fire != null && fire.IsPressed())
            {
                _weapon.TryFire();
            }
        }

        // По дефолту считаем, что рука правая
        private InputAction FireActionOfTheHandHolding()
        {
            var holder = _interactable.firstInteractorSelecting;
            var reference = holder != null && holder.handedness == InteractorHandedness.Left
                ? _fireLeft
                : _fireRight;

            return reference != null ? reference.action : null;
        }
    }
}