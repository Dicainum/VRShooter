using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using VRTraining.Core.Events;
using VRTraining.Core.Services;
using VRTraining.Scenario.Runtime;

namespace VRTraining.UI
{
    // Нотиф о новой группе и о нарушениях
    public sealed class NotificationPanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Text _title;
        [SerializeField] private Text _body;

        [SerializeField] private PlayerFacingUi _anchor;

        [SerializeField] private float _visibleSeconds = 5f;
        [SerializeField] private float _fadeSeconds = 0.6f;

        private IEventBus _bus;
        private Action<GroupStartedEvent> _onGroupStarted;
        private Action<ScenarioViolationEvent> _onViolation;
        private float _hideAtTime;

        private void Awake()
        {
            _bus = ServiceLocator.Get<IEventBus>();

            _onGroupStarted = OnGroupStarted;
            _onViolation = OnViolation;

            _bus.Subscribe(_onGroupStarted);
            _bus.Subscribe(_onViolation);

            SetAlpha(0f);
        }

        private void OnDestroy()
        {
            if (_bus == null)
            {
                return;
            }

            _bus.Unsubscribe(_onGroupStarted);
            _bus.Unsubscribe(_onViolation);
        }

        private void Update()
        {
            if (_canvasGroup == null || _canvasGroup.alpha <= 0f)
            {
                return;
            }

            var remaining = _hideAtTime - Time.time;
            if (remaining <= 0f)
            {
                SetAlpha(0f);
                return;
            }

            if (remaining < _fadeSeconds)
            {
                SetAlpha(remaining / _fadeSeconds);
            }
        }

        public void Show(string title, string body)
        {
            if (_title != null)
            {
                _title.text = title;
            }

            if (_body != null)
            {
                _body.text = body;
            }

            // Игрок может стоять к панели спиной, поэтому сначала переносим её под взгляд
            if (_anchor != null)
            {
                _anchor.ShowInFrontOfPlayer();
            }

            _hideAtTime = Time.time + _visibleSeconds;
            SetAlpha(1f);
        }

        private void OnGroupStarted(GroupStartedEvent evt)
        {
            var builder = new StringBuilder("Порядок действий:");
            for (var i = 0; i < evt.Group.Steps.Count; i++)
            {
                builder.Append('\n')
                    .Append(i + 1)
                    .Append(". ")
                    .Append(evt.Group.Steps[i].Definition.Description);
            }

            Show($"Группа {evt.Number}/{evt.TotalGroups}: {evt.Group.Definition.Name}", builder.ToString());
        }

        private void OnViolation(ScenarioViolationEvent evt)
        {
            var expected = evt.ExpectedStep != null ? evt.ExpectedStep.Definition.Description : string.Empty;

            if (evt.Kind == ViolationKind.SequenceBroken)
            {
                Show("Нарушен порядок", $"Группа закрыта досрочно.\nОжидался шаг: {expected}");
                return;
            }

            Show("Не та цель", $"Ожидался шаг: {expected}");
        }

        private void SetAlpha(float alpha)
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = alpha;
            }
        }
    }
}