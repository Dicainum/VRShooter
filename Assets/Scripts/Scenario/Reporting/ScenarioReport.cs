using System.Collections.Generic;
using VRTraining.Scenario.Data;

namespace VRTraining.Scenario.Reporting
{
    public readonly struct StepResult
    {
        public StepResult(int id, string groupName, string description, StepStatus status)
        {
            Id = id;
            GroupName = groupName;
            Description = description;
            Status = status;
        }

        public int Id { get; }

        public string GroupName { get; }

        public string Description { get; }

        public StepStatus Status { get; }
    }

    // Плоский слепок прогона, экран итогов не лезет в живое состояние которое уже могли сбросить
    public sealed class ScenarioReport
    {
        public ScenarioReport(string scenarioName, IReadOnlyList<StepResult> steps, float durationSeconds)
        {
            ScenarioName = scenarioName;
            Steps = steps;
            DurationSeconds = durationSeconds;

            foreach (var step in steps)
            {
                switch (step.Status)
                {
                    case StepStatus.Success:
                        SuccessCount++;
                        break;
                    case StepStatus.Error:
                        ErrorCount++;
                        break;
                    case StepStatus.Skipped:
                        SkippedCount++;
                        break;
                }
            }
        }

        public string ScenarioName { get; }

        public IReadOnlyList<StepResult> Steps { get; }

        public float DurationSeconds { get; }

        public int SuccessCount { get; }

        public int ErrorCount { get; }

        public int SkippedCount { get; }

        public int TotalCount => Steps.Count;

        public bool IsPerfect => SuccessCount == TotalCount;
    }
}
