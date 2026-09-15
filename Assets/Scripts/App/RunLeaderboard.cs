using System;
using System.Collections.Generic;
using UnityEngine;
using VRTraining.Core.Platform;

namespace VRTraining.App
{
    [Serializable]
    public struct RunRecord
    {
        public float Seconds;
        public string Platform;
        public string Device;
    }

    // Таблица лучших прогонов в PlayerPrefs: одна json строка, без базы и без файлов рядом с билдом
    public static class RunLeaderboard
    {
        public const int Capacity = 10;

        private const string Key = "vrtraining.leaderboard";

        public static IReadOnlyList<RunRecord> Top()
        {
            return Load();
        }

        public static void Add(float seconds)
        {
            var records = Load();
            records.Add(new RunRecord
            {
                Seconds = seconds,
                Platform = DeviceInfo.PlatformName,
                Device = DeviceInfo.DeviceName
            });

            records.Sort((left, right) => left.Seconds.CompareTo(right.Seconds));
            if (records.Count > Capacity)
            {
                records.RemoveRange(Capacity, records.Count - Capacity);
            }

            PlayerPrefs.SetString(Key, JsonUtility.ToJson(new Storage { Records = records.ToArray() }));
            PlayerPrefs.Save();
        }

        public static void Clear()
        {
            PlayerPrefs.DeleteKey(Key);
            PlayerPrefs.Save();
        }

        private static List<RunRecord> Load()
        {
            var raw = PlayerPrefs.GetString(Key, string.Empty);
            if (string.IsNullOrEmpty(raw))
            {
                return new List<RunRecord>();
            }

            // Поехавший ключ не должен ронять экран итогов, худшее что будет - пустая таблица
            try
            {
                var storage = JsonUtility.FromJson<Storage>(raw);

                return storage?.Records != null ? new List<RunRecord>(storage.Records) : new List<RunRecord>();
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[Leaderboard] Не читается сохранённая таблица: {exception.Message}");

                return new List<RunRecord>();
            }
        }

        [Serializable]
        private sealed class Storage
        {
            public RunRecord[] Records;
        }
    }
}
