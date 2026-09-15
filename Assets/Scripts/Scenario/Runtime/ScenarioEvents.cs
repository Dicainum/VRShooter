using UnityEngine;
using VRTraining.Core.Events;
using VRTraining.Scenario.Data;
using VRTraining.Scenario.Reporting;

namespace VRTraining.Scenario.Runtime
{
    public readonly struct PlayerActionPerformedEvent : IEvent
    {
        public PlayerActionPerformedEvent(ExpectedAction action, string targetId, Vector3 worldPosition)
        {
            Action = action;
            TargetId = targetId;
            WorldPosition = worldPosition;
        }

        public ExpectedAction Action { get; }

        public string TargetId { get; }

        public Vector3 WorldPosition { get; }
    }

    public readonly struct ScenarioStartedEvent : IEvent
    {
        public ScenarioStartedEvent(ScenarioState state)
        {
            State = state;
        }

        public ScenarioState State { get; }
    }

    public readonly struct GroupStartedEvent : IEvent
    {
        public GroupStartedEvent(GroupState group, int number, int totalGroups)
        {
            Group = group;
            Number = number;
            TotalGroups = totalGroups;
        }

        public GroupState Group { get; }

        public int Number { get; }

        public int TotalGroups { get; }
    }

    public readonly struct GroupClosedEvent : IEvent
    {
        public GroupClosedEvent(GroupState group)
        {
            Group = group;
        }

        public GroupState Group { get; }
    }

    public readonly struct StepActivatedEvent : IEvent
    {
        public StepActivatedEvent(StepState step)
        {
            Step = step;
        }

        public StepState Step { get; }
    }

    public readonly struct StepResolvedEvent : IEvent
    {
        public StepResolvedEvent(StepState step)
        {
            Step = step;
        }

        public StepState Step { get; }

        public StepStatus Status => Step.Status;
    }

    public enum ViolationKind
    {
        WrongTarget,
        SequenceBroken
    }

    public readonly struct ScenarioViolationEvent : IEvent
    {
        public ScenarioViolationEvent(ViolationKind kind, ExpectedAction action, string actualTargetId,
            StepState expectedStep)
        {
            Kind = kind;
            Action = action;
            ActualTargetId = actualTargetId;
            ExpectedStep = expectedStep;
        }

        public ViolationKind Kind { get; }

        public ExpectedAction Action { get; }

        public string ActualTargetId { get; }

        public StepState ExpectedStep { get; }
    }

    public readonly struct ScenarioCompletedEvent : IEvent
    {
        public ScenarioCompletedEvent(ScenarioReport report)
        {
            Report = report;
        }

        public ScenarioReport Report { get; }
    }
}