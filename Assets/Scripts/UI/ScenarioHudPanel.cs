using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTraining.Core.Events;
using VRTraining.Core.Services;
using VRTraining.Scenario.Runtime;

namespace VRTraining.UI
{
    public sealed class ScenarioHudPanel : MonoBehaviour
    {
        [SerializeField] private Text _groupTitle;
        [SerializeField] private RectTransform _stepsContainer;
        [SerializeField] private int _stepFontSize = 20;

        private readonly List<Text> _rows = new List<Text>();

        private IEventBus _bus;
        private GroupState _group;

        private Action<GroupStartedEvent> _onGroupStarted;
        private Action<StepActivatedEvent> _onStepActivated;
        private Action<StepResolvedEvent> _onStepResolved;
        private Action<ScenarioCompletedEvent> _onCompleted;

        private void Awake()
        {
            _bus = ServiceLocator.Get<IEventBus>();

            _onGroupStarted = OnGroupStarted;
            _onStepActivated = _ => Refresh();
            _onStepResolved = _ => Refresh();
            _onCompleted = _ => OnScenarioCompleted();

            _bus.Subscribe(_onGroupStarted);
            _bus.Subscribe(_onStepActivated);
            _bus.Subscribe(_onStepResolved);
            _bus.Subscribe(_onCompleted);
        }

        private void OnDestroy()
        {
            if (_bus == null)
            {
                return;
            }

            _bus.Unsubscribe(_onGroupStarted);
            _bus.Unsubscribe(_onStepActivated);
            _bus.Unsubscribe(_onStepResolved);
            _bus.Unsubscribe(_onCompleted);
        }

        private void OnGroupStarted(GroupStartedEvent evt)
        {
            _group = evt.Group;

            if (_groupTitle != null)
            {
                _groupTitle.text = $"Группа {evt.Number}/{evt.TotalGroups}: {evt.Group.Definition.Name}";
            }

            BuildRows(evt.Group.Steps.Count);
            Refresh();
        }

        private void OnScenarioCompleted()
        {
            _group = null;

            if (_groupTitle != null)
            {
                _groupTitle.text = "Тренировка завершена";
            }

            BuildRows(0);
        }

        private void BuildRows(int count)
        {
            while (_rows.Count < count)
            {
                var row = UiFactory.CreateText($"Step_{_rows.Count}", _stepsContainer, string.Empty, _stepFontSize);
                row.gameObject.AddComponent<LayoutElement>().minHeight = _stepFontSize + 12;
                _rows.Add(row);
            }

            for (var i = 0; i < _rows.Count; i++)
            {
                _rows[i].gameObject.SetActive(i < count);
            }
        }

        private void Refresh()
        {
            if (_group == null)
            {
                return;
            }

            for (var i = 0; i < _group.Steps.Count && i < _rows.Count; i++)
            {
                var step = _group.Steps[i];
                var isActive = !_group.IsClosed && i == _group.CurrentStepIndex;
                var marker = isActive ? StepStatusFormatter.ActiveMarker : StepStatusFormatter.InactiveMarker;

                var row = _rows[i];
                row.text = step.IsPending && !isActive
                    ? $"{marker} {step.Definition.Description}"
                    : $"{marker} {step.Definition.Description}  {StepStatusFormatter.Label(step.Status)}";
                row.color = isActive && step.IsPending
                    ? UiFactory.AccentColor
                    : StepStatusFormatter.Color(step.Status);
                row.fontStyle = isActive ? FontStyle.Bold : FontStyle.Normal;
            }
        }
    }
}