using System.Collections.Generic;

namespace VRTraining.Scenario.Data
{
    // Разобранный конфиг неизменяем
    public sealed class StepDefinition
    {
        public StepDefinition(int id, string description, ExpectedAction action, string targetId)
        {
            Id = id;
            Description = description;
            Action = action;
            TargetId = targetId;
        }

        public int Id { get; }

        public string Description { get; }

        public ExpectedAction Action { get; }

        public string TargetId { get; }
    }

    public sealed class GroupDefinition
    {
        public GroupDefinition(int id, string name, IReadOnlyList<StepDefinition> steps)
        {
            Id = id;
            Name = name;
            Steps = steps;
        }

        public int Id { get; }

        public string Name { get; }

        public IReadOnlyList<StepDefinition> Steps { get; }
    }

    public sealed class ScenarioDefinition
    {
        public ScenarioDefinition(string id, string name, IReadOnlyList<GroupDefinition> groups)
        {
            Id = id;
            Name = name;
            Groups = groups;
        }

        public string Id { get; }

        public string Name { get; }

        public IReadOnlyList<GroupDefinition> Groups { get; }

        public int TotalStepCount
        {
            get
            {
                var total = 0;
                for (var i = 0; i < Groups.Count; i++)
                {
                    total += Groups[i].Steps.Count;
                }

                return total;
            }
        }
    }
}