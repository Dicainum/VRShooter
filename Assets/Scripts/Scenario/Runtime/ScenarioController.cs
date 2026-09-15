using System;
using UnityEngine;
using VRTraining.Core.Events;
using VRTraining.Core.Services;
using VRTraining.Scenario.Data;

namespace VRTraining.Scenario.Runtime
{
    // грузит конфиг и перекладывает события игрока в стейт машину
    public sealed class ScenarioController : MonoBehaviour
    {
        [Header("Config")] [SerializeField] private string _streamingAssetsPath = "Scenarios/combat_training_01.json";

        [SerializeField] private bool _runOnStart = true;

        private IEventBus _bus;
        private ScenarioRunner _runner;
        private Action<PlayerActionPerformedEvent> _actionHandler;

        public ScenarioRunner Runner => _runner;

        private void Awake()
        {
            _bus = ServiceLocator.Get<IEventBus>();
            _runner = new ScenarioRunner(_bus);
            _actionHandler = OnPlayerAction;
            _bus.Subscribe(_actionHandler);
        }

        private void Start()
        {
            if (_runOnStart)
            {
                Restart();
            }
        }

        private void OnDestroy()
        {
            if (_bus != null && _actionHandler != null)
            {
                _bus.Unsubscribe(_actionHandler);
            }
        }

        public void Restart()
        {
            ScenarioDefinition definition;
            try
            {
                definition = new StreamingAssetsScenarioSource(_streamingAssetsPath).Load();
            }
            catch (ScenarioConfigException exception)
            {
                Debug.LogError($"[Scenario] {exception.Message}", this);
                return;
            }

            _runner.Run(definition);
        }

        private void OnPlayerAction(PlayerActionPerformedEvent action)
        {
            _runner.HandleAction(action.Action, action.TargetId);
        }
    }
}