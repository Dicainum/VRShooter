using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using VRTraining.Gameplay.Weapons;

namespace VRTraining.Gameplay.Player
{
    public sealed class DesktopInteractor : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private Camera _camera;

        [SerializeField] private Transform _holdAnchor;

        [Header("Input")] [SerializeField] private InputActionReference _point;
        [SerializeField] private InputActionReference _use;
        [SerializeField] private InputActionReference _drop;

        [Header("Interaction")] [SerializeField]
        private float _reach = 10f;

        [SerializeField] private float _aimRange = 60f;
        [SerializeField] private LayerMask _interactionMask = ~0;

        private readonly RaycastHit[] _hits = new RaycastHit[16];

        private bool _fireSuppressedUntilRelease;
        private GameObject _held;
        private Rigidbody _heldBody;
        private Weapon _heldWeapon;
        private RigidbodyInterpolation _heldBodyInterpolation;
        private CollisionDetectionMode _heldBodyCollisionDetection;

        private void Awake()
        {
            if (_camera == null)
            {
                _camera = GetComponentInChildren<Camera>();
            }
        }

        private void Update()
        {
            if (_camera == null)
            {
                return;
            }

            AimHeldWeapon();

            if (WasPressed(_drop))
            {
                Drop();
            }

            if (!IsPressed(_use))
            {
                _fireSuppressedUntilRelease = false;
            }

            // Взаимодействие приоритетней выстрела с оружием в руке ничего не подобрать иначе
            if (WasPressed(_use) && TryInteractAlong(ScreenRay()))
            {
                _fireSuppressedUntilRelease = true;
                return;
            }

            if (_heldWeapon != null && IsPressed(_use) && !_fireSuppressedUntilRelease)
            {
                _heldWeapon.TryFire();
            }
        }

        private Ray ScreenRay()
        {
            var point = _point != null && _point.action != null
                ? _point.action.ReadValue<Vector2>()
                : new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

            return _camera.ScreenPointToRay(point);
        }

        private void AimHeldWeapon()
        {
            if (_heldWeapon == null || _holdAnchor == null)
            {
                return;
            }

            var ray = ScreenRay();
            var aimPoint = CastIgnoringSelf(ray, _aimRange, out var hit)
                ? hit.point
                : ray.GetPoint(_aimRange);

            var toTarget = aimPoint - _heldWeapon.Muzzle.position;
            if (toTarget.sqrMagnitude > 0.0001f)
            {
                _held.transform.rotation = Quaternion.LookRotation(toTarget, Vector3.up);
            }
        }

        public bool TryInteractAlong(Ray ray)
        {
            if (!CastIgnoringSelf(ray, _reach, out var hit))
            {
                return false;
            }

            var grabbable = hit.collider.GetComponentInParent<XRGrabInteractable>();
            if (grabbable == null)
            {
                return false;
            }

            Grab(grabbable);
            return true;
        }

        // Рей стартует из камеры, без фильтра любой взгляд вниз упирается в тело
        private bool CastIgnoringSelf(Ray ray, float distance, out RaycastHit hit)
        {
            hit = default;

            var count = Physics.RaycastNonAlloc(ray, _hits, distance, _interactionMask,
                QueryTriggerInteraction.Ignore);

            var nearest = float.PositiveInfinity;
            var found = false;

            for (var i = 0; i < count; i++)
            {
                var candidate = _hits[i];
                if (candidate.distance >= nearest ||
                    candidate.collider.GetComponentInParent<PlayerMarker>() != null)
                {
                    continue;
                }

                nearest = candidate.distance;
                hit = candidate;
                found = true;
            }

            return found;
        }

        public void Grab(XRGrabInteractable grabbable)
        {
            if (_held != null)
            {
                Drop();
            }

            _held = grabbable.gameObject;
            _heldWeapon = _held.GetComponent<Weapon>();
            _heldBody = _held.GetComponent<Rigidbody>();

            if (_heldBody != null)
            {
                _heldBodyInterpolation = _heldBody.interpolation;
                _heldBodyCollisionDetection = _heldBody.collisionDetectionMode;


                _heldBody.interpolation = RigidbodyInterpolation.None;

                _heldBody.collisionDetectionMode = CollisionDetectionMode.Discrete;

                _heldBody.isKinematic = true;

                // Чтобы предмет в руке не толкал игрока
                _heldBody.detectCollisions = false;
            }

            _held.transform.SetParent(_holdAnchor, false);
            _held.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        public void Drop()
        {
            if (_held == null)
            {
                return;
            }

            _held.transform.SetParent(null, true);

            if (_heldBody != null)
            {
                _heldBody.isKinematic = false;
                _heldBody.detectCollisions = true;
                _heldBody.collisionDetectionMode = _heldBodyCollisionDetection;
                _heldBody.interpolation = _heldBodyInterpolation;
            }

            _held = null;
            _heldBody = null;
            _heldWeapon = null;
        }

        private static bool IsPressed(InputActionReference reference)
        {
            return reference != null && reference.action != null && reference.action.IsPressed();
        }

        private static bool WasPressed(InputActionReference reference)
        {
            return reference != null && reference.action != null && reference.action.WasPressedThisFrame();
        }
    }
}