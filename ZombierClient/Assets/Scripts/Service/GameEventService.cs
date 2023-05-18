using Prototype.Data;
using Prototype.Model;
using System;
using UnityEngine;

namespace Prototype.Service
{
    public class GameEventService
    {
        #region Events

        public event EventHandler PlayerStartFight;
        public event EventHandler PlayerStopFight;
        public event EventHandler PlayerDeath;
        public event EventHandler PlayerRevive;
        public event EventHandler<PlayerMovedEventArgs> PlayerMoved;
        public event EventHandler PlayerShoot;
        public event EventHandler PlayerShootAnimationEvent;

        public event EventHandler<AttackedEventArgs> Attacked;
        public event EventHandler<DamagedEventArgs> Damaged;
        public event EventHandler<EnemyDeathEventArgs> EnemyDeath;
        public event EventHandler<EnemyDeathEventArgs> EnemyDeathInstant;
        public event EventHandler EnemyPreDestroyed;
        public event EventHandler<EnemyDeathAnimationEventArgs> EnemyDeathAnimationEvent;

        public event EventHandler<EnemyMovedEventArgs> EnemyMoved;
        public event EventHandler<EnemyHitEventArgs> EnemyHit;
        public event EventHandler<EnemyAttackEventArgs> EnemyAttack;
        public event EventHandler<EnemyAttackAnimationEventArgs> EnemyAttackAnimationEvent;

        public event EventHandler<LevelClearedEventArgs> LevelCleared;
        public event EventHandler PlayerEnteredExit;

        // Common game events
        public event EventHandler Reset;
        public event EventHandler GamePause;
        public event EventHandler GameUnpause;

        #endregion

        #region EventArgs

        public class LevelClearedEventArgs : EventArgs
        {
            public bool IsLastLevel;
        }

        public class PlayerMovedEventArgs : EventArgs
        {
            public Vector3 Movement;
        }

        public class AttackedEventArgs : EventArgs
        {
            public IDamaging Attacker;
            public IDamageable Defender;
        }

        public class DamagedEventArgs : EventArgs
        {
            public IdData EntityId;
            public IDamageable DamagedEntity;
            public float DamageValue;
            public bool IsCrit;
        }

        public class EnemyDeathEventArgs : EventArgs
        {
            public IDamageable Entity;
        }

        public class EnemyDeathAnimationEventArgs : EventArgs
        {
            public IdData EntityId;
        }

        public class EnemyMovedEventArgs : EventArgs
        {
            public IdData Id;
            public float Value;
        }

        public class EnemyHitEventArgs : EventArgs
        {
            public IdData EntityId;
            public Vector3 HitDirection;
            public Vector3 HitPosition;
        }

        public class EnemyAttackEventArgs : EventArgs
        {
            public IdData EntityId;
        }

        public class EnemyAttackAnimationEventArgs : EventArgs
        {
            public IdData EntityId;
        }

        #endregion

        #region Invokers

        public void OnGamePause()
        {
            GamePause?.Invoke(this, EventArgs.Empty);
        }

        public void OnGameUnpause()
        {
            GameUnpause?.Invoke(this, EventArgs.Empty);
        }

        public void OnEnemyPreDestroyed()
        {
            EnemyPreDestroyed?.Invoke(this, EventArgs.Empty);
        }

        public void OnLevelCleared(LevelClearedEventArgs e)
        {
            LevelCleared?.Invoke(this, e);
        }

        public void OnPlayerEnteredExit()
        {
            PlayerEnteredExit?.Invoke(this, EventArgs.Empty);
        }

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

        public void OnPlayerRevive()
        {
            PlayerRevive?.Invoke(this, EventArgs.Empty);
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

        public void OnAttacked(AttackedEventArgs e)
        {
            Attacked?.Invoke(this, e);
        }

        public void OnDamaged(DamagedEventArgs e)
        {
            Damaged?.Invoke(this, e);
        }

        public void OnEnemyDeath(EnemyDeathEventArgs e)
        {
            EnemyDeath?.Invoke(this, e);
        }

        public void OnEnemyDeathAnimationEvent(EnemyDeathAnimationEventArgs e)
        {
            EnemyDeathAnimationEvent?.Invoke(this, e);
        }

        public void OnEnemyDeathInstant(EnemyDeathEventArgs e)
        {
            EnemyDeathInstant?.Invoke(this, e);
        }

        public void OnEnemyMoved(EnemyMovedEventArgs e)
        {
            EnemyMoved?.Invoke(this, e);
        }

        public void OnEnemyHit(EnemyHitEventArgs e)
        {
            EnemyHit?.Invoke(this, e);
        }

        public void OnEnemyAttack(EnemyAttackEventArgs e)
        {
            EnemyAttack?.Invoke(this, e);
        }

        public void OnEnemyAttackAnimationEvent(EnemyAttackAnimationEventArgs e)
        {
            EnemyAttackAnimationEvent?.Invoke(this, e);
        }

        public void OnReset()
        {
            Reset?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
