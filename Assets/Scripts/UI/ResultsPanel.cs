using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTraining.App;
using VRTraining.Core.Events;
using VRTraining.Core.Services;
using VRTraining.Scenario.Data;
using VRTraining.Scenario.Reporting;
using VRTraining.Scenario.Runtime;

namespace VRTraining.UI
{
    public sealed class ResultsPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private Text _title;
        [SerializeField] private Text _summary;
        [SerializeField] private RectTransform _rowsContainer;
        [SerializeField] private RectTransform _leaderboardContainer;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _lobbyButton;
        [SerializeField] private PlayerFacingUi _anchor;
        [SerializeField] private int _rowFontSize = 20;

        private readonly List<RectTransform> _rows = new List<RectTransform>();
        private readonly List<RectTransform> _leaderboardRows = new List<RectTransform>();

        private IEventBus _bus;
        private ISceneLoader _sceneLoader;
        private Action<ScenarioCompletedEvent> _onCompleted;

        private void Awake()
        {
            _bus = ServiceLocator.Get<IEventBus>();
            _sceneLoader = ServiceLocator.Get<ISceneLoader>();

            _onCompleted = evt => Show(evt.Report);
            _bus.Subscribe(_onCompleted);

            if (_restartButton != null)
            {
                _restartButton.onClick.AddListener(() => _sceneLoader.ReloadCurrent());
            }

            if (_lobbyButton != null)
            {
                _lobbyButton.onClick.AddListener(() => _sceneLoader.LoadLobby());
            }

            SetVisible(false);
        }

        private void OnDestroy()
        {
            if (_bus != null)
            {
                _bus.Unsubscribe(_onCompleted);
            }
        }

        public void Show(ScenarioReport report)
        {
            if (_title != null)
            {
                _title.text = $"Итоги: {report.ScenarioName}";
            }

            if (_summary != null)
            {
                _summary.text =
                    $"Успешно: {report.SuccessCount}   " +
                    $"С ошибкой: {report.ErrorCount}   " +
                    $"Пропущено: {report.SkippedCount}   " +
                    $"из {report.TotalCount}   •   Время: {FormatDuration(report.DurationSeconds)}";
                _summary.color = report.IsPerfect
                    ? StepStatusFormatter.Color(StepStatus.Success)
                    : UiFactory.TextColor;
            }

            // В таблицу пишем только чистый проход, иначе рекорд можно выбить пропустив половину шагов
            if (report.IsPerfect)
            {
                RunLeaderboard.Add(report.DurationSeconds);
            }

            BuildRows(report);
            BuildLeaderboard();
            SetVisible(true);

            // Итоги без этого могут открыться за спиной игрока
            if (_anchor != null)
            {
                _anchor.ShowInFrontOfPlayer();
            }
        }

        private static string FormatDuration(float seconds)
        {
            var span = TimeSpan.FromSeconds(Mathf.Max(0f, seconds));

            return $"{(int)span.TotalMinutes:00}:{span.Seconds:00}";
        }

        private void BuildRows(ScenarioReport report)
        {
            while (_rows.Count < report.Steps.Count)
            {
                _rows.Add(CreateRow(_rows.Count));
            }

            for (var i = 0; i < _rows.Count; i++)
            {
                var visible = i < report.Steps.Count;
                _rows[i].gameObject.SetActive(visible);
                if (!visible)
                {
                    continue;
                }

                var step = report.Steps[i];
                var description = _rows[i].GetChild(0).GetComponent<Text>();
                var status = _rows[i].GetChild(1).GetComponent<Text>();

                description.text = $"{step.Id}. {step.Description}";
                status.text = StepStatusFormatter.Label(step.Status);
                status.color = StepStatusFormatter.Color(step.Status);
            }
        }

        private RectTransform CreateRow(int index)
        {
            var row = UiFactory.CreateRect($"Row_{index}", _rowsContainer);

            var layoutGroup = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = true;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.spacing = 12f;

            row.gameObject.AddComponent<LayoutElement>().minHeight = _rowFontSize + 14;

            var description = UiFactory.CreateText("Description", row, string.Empty, _rowFontSize,
                TextAnchor.MiddleLeft);
            description.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1f;

            var status = UiFactory.CreateText("Status", row, string.Empty, _rowFontSize, TextAnchor.MiddleRight);
            var statusLayout = status.gameObject.AddComponent<LayoutElement>();
            statusLayout.preferredWidth = 190f;
            statusLayout.flexibleWidth = 0f;

            return row;
        }

        private void BuildLeaderboard()
        {
            if (_leaderboardContainer == null)
            {
                return;
            }

            var records = RunLeaderboard.Top();

            while (_leaderboardRows.Count < records.Count)
            {
                _leaderboardRows.Add(CreateLeaderboardRow(_leaderboardRows.Count));
            }

            for (var i = 0; i < _leaderboardRows.Count; i++)
            {
                var visible = i < records.Count;
                _leaderboardRows[i].gameObject.SetActive(visible);
                if (!visible)
                {
                    continue;
                }

                var record = records[i];
                _leaderboardRows[i].GetChild(0).GetComponent<Text>().text = $"{i + 1}.";
                _leaderboardRows[i].GetChild(1).GetComponent<Text>().text = FormatDuration(record.Seconds);
                _leaderboardRows[i].GetChild(2).GetComponent<Text>().text = record.Platform;
                _leaderboardRows[i].GetChild(3).GetComponent<Text>().text = record.Device;
            }
        }

        private RectTransform CreateLeaderboardRow(int index)
        {
            var row = UiFactory.CreateRect($"Record_{index}", _leaderboardContainer);

            var layoutGroup = row.gameObject.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = true;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.spacing = 10f;

            row.gameObject.AddComponent<LayoutElement>().minHeight = _rowFontSize + 12;

            Cell(row, "Place", 52f, TextAnchor.MiddleRight);
            Cell(row, "Time", 96f, TextAnchor.MiddleRight);
            Cell(row, "Platform", 140f, TextAnchor.MiddleLeft);
            Cell(row, "Device", 0f, TextAnchor.MiddleLeft).gameObject
                .GetComponent<LayoutElement>().flexibleWidth = 1f;

            return row;
        }

        private Text Cell(Transform row, string name, float width, TextAnchor alignment)
        {
            var text = UiFactory.CreateText(name, row, string.Empty, _rowFontSize, alignment);
            var layout = text.gameObject.AddComponent<LayoutElement>();
            layout.preferredWidth = width;
            layout.flexibleWidth = 0f;

            return text;
        }

        private void SetVisible(bool visible)
        {
            if (_root != null)
            {
                _root.SetActive(visible);
            }
        }
    }
}
