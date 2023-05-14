using System;

namespace Prototype.Service
{
    public class GameUIEventService
    {
        #region Events
        public event EventHandler CameraOnDeadPlayer;
        #endregion

        #region EventArgs
        #endregion

        #region Invokers
        public void OnCameraOnDeadPlayer()
        {
            CameraOnDeadPlayer?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
