using System;

namespace VRTraining.Scenario.Data
{
    // Имена полей повторяют ключи json буква в букву
    [Serializable]
    public sealed class ScenarioConfigDto
    {
        public string ScenarioId;
        public string ScenarioName;
        public GroupDto[] Groups;
    }

    [Serializable]
    public sealed class GroupDto
    {
        public int GroupId;
        public string GroupName;
        public StepDto[] Steps;
    }

    [Serializable]
    public sealed class StepDto
    {
        public int id;
        public string description;
        public string expectedAction;
        public string target;
    }
}