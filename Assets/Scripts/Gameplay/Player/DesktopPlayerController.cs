using UnityEngine;
using UnityEngine.InputSystem;

namespace VRTraining.Gameplay.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class DesktopPlayerController : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private Transform _head;

        [Header("Input")] [SerializeField] private InputActionReference _move;
        [SerializeField] private InputActionReference _sprint;
        [SerializeField] private InputActionReference _look;
        [SerializeField] private InputActionReference _lookHold;

        [Header("Movement")] [SerializeField] private float _walkSpeed = 2.8f;
        [SerializeField] private float _sprintMultiplier = 1.9f;
        [SerializeField] private float _gravity = -9.81f;

        [Header("Look")] [SerializeField] private float _lookSensitivity = 0.12f;
        [SerializeField] private float _maxPitch = 85f;

        private CharacterController _controller;
        private float _pitch;
        private float _verticalVelocity;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();

            if (_head == null && Camera.main != null)
            {
                _head = Camera.main.transform;
            }
        }

        private void Update()
        {
            UpdateLook();
            UpdateMove();
        }

        private void UpdateLook()
        {
            if (_head == null || !IsPressed(_lookHold))
            {
                return;
            }

            var delta = ReadVector2(_look) * _lookSensitivity;

            transform.Rotate(Vector3.up, delta.x, Space.World);

            _pitch = Mathf.Clamp(_pitch - delta.y, -_maxPitch, _maxPitch);
            var headAngles = _head.localEulerAngles;
            _head.localRotation = Quaternion.Euler(_pitch, headAngles.y, 0f);
        }

        private void UpdateMove()
        {
            var input = Vector2.ClampMagnitude(ReadVector2(_move), 1f);

            var speed = _walkSpeed * (IsPressed(_sprint) ? _sprintMultiplier : 1f);
            var move = (transform.right * input.x + transform.forward * input.y) * speed;

            // Прижим вниз, isGrounded скачет на плоском полу
            _verticalVelocity = _controller.isGrounded
                ? -1f
                : _verticalVelocity + _gravity * Time.deltaTime;

            move.y = _verticalVelocity;
            _controller.Move(move * Time.deltaTime);
        }

        private static Vector2 ReadVector2(InputActionReference reference)
        {
            return reference != null && reference.action != null
                ? reference.action.ReadValue<Vector2>()
                : Vector2.zero;
        }

        private static bool IsPressed(InputActionReference reference)
        {
            return reference != null && reference.action != null && reference.action.IsPressed();
        }
    }
}