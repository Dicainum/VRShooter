using UnityEngine;
using VRTraining.Core.Events;
using VRTraining.Core.Services;

namespace VRTraining.App
{
    // Сервисы поднимаются до загрузки сцены, чтобы любая сцена запускалась с редактора напрямую
    public static class AppBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            var root = new GameObject("[App]");
            Object.DontDestroyOnLoad(root);

            ServiceLocator.Register<IEventBus>(new EventBus());
            ServiceLocator.Register<ISceneLoader>(root.AddComponent<SceneLoaderService>());
        }
    }
}
