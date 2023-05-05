using Prototype.Model;
using System;
using UnityEngine;

namespace Prototype.Service
{
    public class GameplayEventService
    {
        #region Events
        public event EventHandler PlayerStartFight;
        public event EventHandler PlayerStopFight;
        public event EventHandler PlayerDeath;
        public event EventHandler<PlayerMovedEventArgs> PlayerMoved;
        public event EventHandler PlayerShoot;
        public event EventHandler PlayerShootAnimationEvent;

        public event EventHandler<DamagedEventArgs> Damaged;
        public event EventHandler<DeathEventArgs> Death;
        #endregion

        #region EventArgs
        public class PlayerMovedEventArgs : EventArgs
        {
            public Vector3 Movement;
        }

        public class DamagedEventArgs : EventArgs
        {
            public IDamaging Attacker;
            public IDamageable Defender;
        }

        public class DeathEventArgs : EventArgs
        {
            public IDamageable Entity;
        }

        #endregion

        #region Invokers
        public void OnPlayerStartFight()
        {
            PlayerStartFight?.Invoke(this, EventArgs.Empty);
        }

        public void OnPlayerStopFight()
        {
            PlayerStopFight?.Invoke(this, EventArgs.Empty);
        }

        public void OnPlayerDeath()
        {
            PlayerDeath?.Invoke(this, EventArgs.Empty);
        }

        public void OnPlayerMoved(PlayerMovedEventArgs e)
        {
            PlayerMoved?.Invoke(this, e);
        }

        public void OnPlayerShoot()
        {
            PlayerShoot?.Invoke(this, EventArgs.Empty);
        }

        public void OnPlayerShootAnimationEvent()
        {
            PlayerShootAnimationEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnDamaged(DamagedEventArgs e)
        {
            Damaged?.Invoke(this, e);
        }

        public void OnDeath(DeathEventArgs e)
        {
            Death?.Invoke(this, e);
        }
        #endregion
    }
}
