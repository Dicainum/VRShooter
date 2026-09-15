using UnityEngine;
using VRTraining.Scenario.Data;

namespace VRTraining.UI
{
    public static class StepStatusFormatter
    {
        public const string ActiveMarker = "►";
        public const string InactiveMarker = "•";

        private static readonly Color PendingColor = new Color(0.72f, 0.76f, 0.80f);
        private static readonly Color SuccessColor = new Color(0.40f, 0.85f, 0.45f);
        private static readonly Color ErrorColor = new Color(0.95f, 0.45f, 0.35f);
        private static readonly Color SkippedColor = new Color(0.60f, 0.60f, 0.65f);

        public static string Label(StepStatus status)
        {
            switch (status)
            {
                case StepStatus.Success:
                    return "[Успешно]";
                case StepStatus.Error:
                    return "[С ошибкой]";
                case StepStatus.Skipped:
                    return "[Пропущен]";
                default:
                    return "[Ожидание]";
            }
        }

        public static Color Color(StepStatus status)
        {
            switch (status)
            {
                case StepStatus.Success:
                    return SuccessColor;
                case StepStatus.Error:
                    return ErrorColor;
                case StepStatus.Skipped:
                    return SkippedColor;
                default:
                    return PendingColor;
            }
        }
    }
}
