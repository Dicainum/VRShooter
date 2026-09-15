namespace VRTraining.App
{
    public interface ISceneLoader
    {
        void LoadLobby();

        void LoadTraining();

        void ReloadCurrent();

        void QuitApplication();
    }
}
