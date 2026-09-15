using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRTraining.Gameplay.Interaction
{
    public static class ScenarioTargetRegistry
    {
        private static readonly Dictionary<string, ScenarioTarget> Targets =
            new Dictionary<string, ScenarioTarget>(StringComparer.OrdinalIgnoreCase);

        public static IReadOnlyDictionary<string, ScenarioTarget> All => Targets;

        public static void Register(ScenarioTarget target)
        {
            var id = target.TargetId;
            if (string.IsNullOrWhiteSpace(id))
            {
                return;
            }

            Targets[id] = target;
        }

        public static void Unregister(ScenarioTarget target)
        {
            var id = target.TargetId;
            if (!string.IsNullOrWhiteSpace(id) && Targets.TryGetValue(id, out var existing) && existing == target)
            {
                Targets.Remove(id);
            }
        }

        public static bool TryGet(string targetId, out ScenarioTarget target)
        {
            if (!string.IsNullOrWhiteSpace(targetId) && Targets.TryGetValue(targetId, out target) && target != null)
            {
                return true;
            }

            target = null;
            return false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnEnterPlayMode()
        {
            Targets.Clear();
        }
    }
}