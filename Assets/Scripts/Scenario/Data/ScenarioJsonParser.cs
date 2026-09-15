using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRTraining.Scenario.Data
{
    public sealed class ScenarioConfigException : Exception
    {
        public ScenarioConfigException(string message) : base(message)
        {
        }
    }

    // Валидация намеренно строгая, битый конфиг лучше поймать на загрузке чем потом
    public static class ScenarioJsonParser
    {
        public static ScenarioDefinition Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ScenarioConfigException("Конфиг сценария пустой");
            }

            ScenarioConfigDto dto;
            try
            {
                dto = JsonUtility.FromJson<ScenarioConfigDto>(json);
            }
            catch (Exception exception)
            {
                throw new ScenarioConfigException($"Сломанный json сценария: {exception.Message}");
            }

            if (dto == null || string.IsNullOrWhiteSpace(dto.ScenarioId))
            {
                throw new ScenarioConfigException("В конфиге нет 'ScenarioId'");
            }

            if (dto.Groups == null || dto.Groups.Length == 0)
            {
                throw new ScenarioConfigException($"В сценарии '{dto.ScenarioId}' нет ни одной группы");
            }

            var seenStepIds = new HashSet<int>();
            var groups = new List<GroupDefinition>(dto.Groups.Length);

            foreach (var groupDto in dto.Groups)
            {
                groups.Add(ParseGroup(dto.ScenarioId, groupDto, seenStepIds));
            }

            var name = string.IsNullOrWhiteSpace(dto.ScenarioName) ? dto.ScenarioId : dto.ScenarioName;

            return new ScenarioDefinition(dto.ScenarioId, name, groups);
        }

        private static GroupDefinition ParseGroup(string scenarioId, GroupDto groupDto, ISet<int> seenStepIds)
        {
            if (groupDto == null)
            {
                throw new ScenarioConfigException($"В сценарии '{scenarioId}' пустая запись группы");
            }

            if (groupDto.Steps == null || groupDto.Steps.Length == 0)
            {
                throw new ScenarioConfigException($"В группе {groupDto.GroupId} нет шагов");
            }

            var steps = new List<StepDefinition>(groupDto.Steps.Length);
            foreach (var stepDto in groupDto.Steps)
            {
                steps.Add(ParseStep(groupDto, stepDto, seenStepIds));
            }

            var groupName = string.IsNullOrWhiteSpace(groupDto.GroupName)
                ? $"Группа {groupDto.GroupId}"
                : groupDto.GroupName;

            return new GroupDefinition(groupDto.GroupId, groupName, steps);
        }

        private static StepDefinition ParseStep(GroupDto groupDto, StepDto stepDto, ISet<int> seenStepIds)
        {
            if (stepDto == null)
            {
                throw new ScenarioConfigException($"В группе {groupDto.GroupId} пустая запись шага");
            }

            // Id шага уходит в отчет, дубликат там превратится в две неразличимые строки
            if (!seenStepIds.Add(stepDto.id))
            {
                throw new ScenarioConfigException($"Id шага {stepDto.id} встречается больше одного раза");
            }

            if (!TryParseAction(stepDto.expectedAction, out var action))
            {
                throw new ScenarioConfigException(
                    $"Шаг {stepDto.id}: неизвестное действие '{stepDto.expectedAction}'. " +
                    $"Допустимые: {string.Join(", ", Enum.GetNames(typeof(ExpectedAction)))}");
            }

            if (string.IsNullOrWhiteSpace(stepDto.target))
            {
                throw new ScenarioConfigException($"У шага {stepDto.id} не указан 'target'");
            }

            var description = string.IsNullOrWhiteSpace(stepDto.description)
                ? $"Шаг {stepDto.id}"
                : stepDto.description;

            return new StepDefinition(stepDto.id, description, action, stepDto.target.Trim());
        }

        private static bool TryParseAction(string raw, out ExpectedAction action)
        {
            action = default;
            var name = raw?.Trim();

            // Enum.TryParse глотает и числа, но "3" в конфиге это опечатка а не Shoot
            return !string.IsNullOrEmpty(name) && char.IsLetter(name[0]) && Enum.TryParse(name, true, out action);
        }
    }
}
