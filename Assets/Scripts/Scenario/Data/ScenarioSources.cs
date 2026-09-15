using System.IO;
using UnityEngine;

namespace VRTraining.Scenario.Data
{
    public sealed class StreamingAssetsScenarioSource
    {
        private readonly string _relativePath;

        public StreamingAssetsScenarioSource(string relativePath)
        {
            _relativePath = relativePath;
        }

        public string AbsolutePath => Path.Combine(Application.streamingAssetsPath, _relativePath);

        public ScenarioDefinition Load()
        {
            var path = AbsolutePath;
            if (!File.Exists(path))
            {
                throw new ScenarioConfigException($"Конфиг сценария не найден: '{path}'");
            }

            return ScenarioJsonParser.Parse(File.ReadAllText(path));
        }
    }
}
