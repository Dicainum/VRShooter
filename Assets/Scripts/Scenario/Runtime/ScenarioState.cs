using System.Collections.Generic;
using VRTraining.Scenario.Data;

namespace VRTraining.Scenario.Runtime
{
    public sealed class StepState
    {
        internal StepState(StepDefinition definition, GroupState group)
        {
            Definition = definition;
            Group = group;
            Status = StepStatus.Pending;
        }

        public StepDefinition Definition { get; }

        public GroupState Group { get; }

        public StepStatus Status { get; private set; }

        public bool IsPending => Status == StepStatus.Pending;

        internal void Resolve(StepStatus status)
        {
            Status = status;
        }
    }

    public sealed class GroupState
    {
        private readonly List<StepState> _steps;

        internal GroupState(GroupDefinition definition)
        {
            Definition = definition;
            _steps = new List<StepState>(definition.Steps.Count);

            foreach (var step in definition.Steps)
            {
                _steps.Add(new StepState(step, this));
            }
        }

        public GroupDefinition Definition { get; }

        public IReadOnlyList<StepState> Steps => _steps;

        public int CurrentStepIndex { get; internal set; }

        public bool IsClosed { get; internal set; }

        public StepState CurrentStep =>
            IsClosed || CurrentStepIndex < 0 || CurrentStepIndex >= _steps.Count
                ? null
                : _steps[CurrentStepIndex];
    }

    // Состояние собирается заново на каждый прогон, разобранный конфиг переиспользуется как есть
    public sealed class ScenarioState
    {
        private readonly List<GroupState> _groups;

        public ScenarioState(ScenarioDefinition definition)
        {
            Definition = definition;
            _groups = new List<GroupState>(definition.Groups.Count);

            foreach (var group in definition.Groups)
            {
                _groups.Add(new GroupState(group));
            }

            CurrentGroupIndex = -1;
        }

        public ScenarioDefinition Definition { get; }

        public IReadOnlyList<GroupState> Groups => _groups;

        public int CurrentGroupIndex { get; internal set; }

        public bool IsFinished { get; internal set; }

        public GroupState CurrentGroup =>
            CurrentGroupIndex < 0 || CurrentGroupIndex >= _groups.Count ? null : _groups[CurrentGroupIndex];

        public IEnumerable<StepState> AllSteps
        {
            get
            {
                foreach (var group in _groups)
                {
                    foreach (var step in group.Steps)
                    {
                        yield return step;
                    }
                }
            }
        }
    }
}