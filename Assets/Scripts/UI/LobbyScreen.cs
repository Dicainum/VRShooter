using UnityEngine;
using UnityEngine.UI;
using VRTraining.App;
using VRTraining.Core.Services;

namespace VRTraining.UI
{
    public sealed class LobbyScreen : MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _quitButton;

        private ISceneLoader _sceneLoader;

        private void Awake()
        {
            _sceneLoader = ServiceLocator.Get<ISceneLoader>();

            if (_startButton != null)
            {
                _startButton.onClick.AddListener(StartTraining);
            }

            if (_quitButton != null)
            {
                _quitButton.onClick.AddListener(Quit);
            }
        }

        public void StartTraining()
        {
            _sceneLoader.LoadTraining();
        }

        public void Quit()
        {
            _sceneLoader.QuitApplication();
        }
    }
}
