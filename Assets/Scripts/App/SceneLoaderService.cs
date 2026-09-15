using UnityEngine;
using UnityEngine.SceneManagement;

namespace VRTraining.App
{
    public sealed class SceneLoaderService : MonoBehaviour, ISceneLoader
    {
        public void LoadLobby()
        {
            SceneManager.LoadScene(SceneNames.Lobby);
        }

        public void LoadTraining()
        {
            SceneManager.LoadScene(SceneNames.Training);
        }

        public void ReloadCurrent()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void QuitApplication()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
