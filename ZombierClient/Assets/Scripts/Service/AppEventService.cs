using System;

namespace Prototype.Service
{
    public class AppEventService
    {
        #region Events

        public event EventHandler CameraOnDeadPlayer;
        public event EventHandler<LoadSceneEventArgs> LoadScene;

        #endregion

        #region Invokers

        public void OnLoadScene(LoadSceneEventArgs e)
        {
            LoadScene?.Invoke(this, e);
        }

        public void OnCameraOnDeadPlayer()
        {
            CameraOnDeadPlayer?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }

    #region EventArgs

    public class LoadSceneEventArgs : EventArgs
    {
        public Scene To;
    }

    #endregion
}
