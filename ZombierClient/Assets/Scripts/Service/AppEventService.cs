using Prototype.Data;
using System;

namespace Prototype.Service
{
    public class AppEventService
    {
        #region Events

        public event EventHandler CameraOnDeadPlayer;
        public event EventHandler<LoadSceneEventArgs> LoadScene;
        public event EventHandler GamePause;
        public event EventHandler GameUnpause;
        public event EventHandler<PlayEventArgs> Play;
        public event EventHandler<PlayerPassedLevelEventArgs> PlayerPassedLevel;

        public event EventHandler UserHasUnfinishedGameSession;
        public event EventHandler ResumeGameSession;
        public event EventHandler DontResumeGameSession;

        #endregion

        #region Invokers

        public void OnResumeGameSession()
        {
            ResumeGameSession?.Invoke(this, EventArgs.Empty);
        }

        public void OnDontResumeGameSession()
        {
            DontResumeGameSession?.Invoke(this, EventArgs.Empty);
        }

        public void OnUserHasUnfinishedGameSession()
        {
            UserHasUnfinishedGameSession?.Invoke(this, EventArgs.Empty);
        }

        public void OnPlayerPassedLevel(PlayerPassedLevelEventArgs e)
        {
            PlayerPassedLevel?.Invoke(this, e);
        }

        public void OnPlay(PlayEventArgs e)
        {
            Play?.Invoke(this, e);
        }

        public void OnLoadScene(LoadSceneEventArgs e)
        {
            LoadScene?.Invoke(this, e);
        }

        public void OnCameraOnDeadPlayer()
        {
            CameraOnDeadPlayer?.Invoke(this, EventArgs.Empty);
        }

        public void OnGamePause()
        {
            GamePause?.Invoke(this, EventArgs.Empty);
        }

        public void OnGameUnpause()
        {
            GameUnpause?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }

    #region EventArgs

    public class PlayerPassedLevelEventArgs : EventArgs
    {
        public GameSessionData GameSession;
    }

    public class LoadSceneEventArgs : EventArgs
    {
        public Scene To;
    }

    public class PlayEventArgs : EventArgs
    {
        public LocationData LocationData;
    }

    #endregion
}
