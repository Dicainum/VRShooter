using System;
using System.Collections.Generic;
using UnityEngine;
using VRTraining.Core.Events;
using VRTraining.Scenario.Data;
using VRTraining.Scenario.Reporting;

namespace VRTraining.Scenario.Runtime
{
    public sealed class ScenarioRunner
    {
        private readonly IEventBus _bus;

        private float _startedAtRealtime;

        public ScenarioRunner(IEventBus bus)
        {
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        public ScenarioState State { get; private set; }

        public bool IsRunning => State != null && !State.IsFinished;

        public void Run(ScenarioDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            State = new ScenarioState(definition);

            _bus.Publish(new ScenarioStartedEvent(State));
            OpenGroup(0);
        }

        public void HandleAction(ExpectedAction action, string targetId)
        {
            if (!IsRunning)
            {
                return;
            }

            var group = State.CurrentGroup;
            var activeStep = group?.CurrentStep;
            if (activeStep == null)
            {
                return;
            }

            if (Matches(activeStep, action, targetId))
            {
                ResolveStep(activeStep, StepStatus.Success);
                AdvanceStep(group);
                return;
            }

            // Забег вперед проверяем раньше промаха по цели, иначе досрочный правильный граб олтчитается как обычная ошибка, а это именно нарушение порядка
            for (var i = group.CurrentStepIndex + 1; i < group.Steps.Count; i++)
            {
                var laterStep = group.Steps[i];
                if (laterStep.IsPending && Matches(laterStep, action, targetId))
                {
                    HandleSequenceViolation(group, action, targetId, activeStep);
                    return;
                }
            }

            if (activeStep.Definition.Action == action)
            {
                _bus.Publish(new ScenarioViolationEvent(ViolationKind.WrongTarget, action, targetId, activeStep));
                ResolveStep(activeStep, StepStatus.Error);
                AdvanceStep(group);
                return;
            }

            // Все остальное к сценарию не относится и не штрафуется. Подобрал по дороге чужую вещь - право игрока
        }

        public ScenarioReport BuildReport()
        {
            var rows = new List<StepResult>(State.Definition.TotalStepCount);
            foreach (var step in State.AllSteps)
            {
                rows.Add(new StepResult(
                    step.Definition.Id,
                    step.Group.Definition.Name,
                    step.Definition.Description,
                    step.Status));
            }

            return new ScenarioReport(State.Definition.Name, rows, Time.realtimeSinceStartup - _startedAtRealtime);
        }

        private static bool Matches(StepState step, ExpectedAction action, string targetId)
        {
            return step.Definition.Action == action &&
                   string.Equals(step.Definition.TargetId, targetId, StringComparison.OrdinalIgnoreCase);
        }

        private void OpenGroup(int index)
        {
            State.CurrentGroupIndex = index;

            var group = State.CurrentGroup;
            if (group == null)
            {
                Complete();
                return;
            }

            group.CurrentStepIndex = 0;

            // Секундомер идёт с первого шага, а не с загрузки сцены
            if (index == 0)
            {
                _startedAtRealtime = Time.realtimeSinceStartup;
            }

            _bus.Publish(new GroupStartedEvent(group, index + 1, State.Groups.Count));
            _bus.Publish(new StepActivatedEvent(group.CurrentStep));
        }

        private void AdvanceStep(GroupState group)
        {
            group.CurrentStepIndex++;

            if (group.CurrentStepIndex >= group.Steps.Count)
            {
                CloseGroup(group);
                return;
            }

            _bus.Publish(new StepActivatedEvent(group.CurrentStep));
        }

        private void HandleSequenceViolation(GroupState group, ExpectedAction action, string targetId,
            StepState expectedStep)
        {
            _bus.Publish(new ScenarioViolationEvent(ViolationKind.SequenceBroken, action, targetId, expectedStep));
            foreach (var step in group.Steps)
            {
                if (step.IsPending)
                {
                    ResolveStep(step, StepStatus.Skipped);
                }
            }

            CloseGroup(group);
        }

        private void CloseGroup(GroupState group)
        {
            group.IsClosed = true;
            _bus.Publish(new GroupClosedEvent(group));

            OpenGroup(State.CurrentGroupIndex + 1);
        }

        private void ResolveStep(StepState step, StepStatus status)
        {
            step.Resolve(status);
            _bus.Publish(new StepResolvedEvent(step));
        }

        private void Complete()
        {
            State.IsFinished = true;
            _bus.Publish(new ScenarioCompletedEvent(BuildReport()));
        }
    }
}