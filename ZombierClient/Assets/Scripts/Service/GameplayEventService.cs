using Prototype.Data;
using Prototype.View;
using System;
using UnityEngine;

namespace Prototype.Service
{
    public class GameplayEventService
    {
        public event EventHandler PlayerStartFight;
        public event EventHandler PlayerStopFight;
        public event EventHandler PlayerDeath;

        public event EventHandler<PlayerMovedEventArgs> PlayerMoved;
        public class PlayerMovedEventArgs : EventArgs
        {
            public Vector3 Movement;
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

        public void OnPlayerMoved(PlayerMovedEventArgs e)
        {
            PlayerMoved?.Invoke(this, e);
        }

        //////////////TODO: Refactor ////////////////////////////
        /// Remove static events and use .Net standard event pattern instead

        public delegate void MovePlayerWalkHandler(in PlayerData sender, in Vector3 move);
        public static event MovePlayerWalkHandler OnMovePlayerWalk;
        public static void InvokeOnMovePlayerWalk(in PlayerData sender, in Vector3 move)
        {
            OnMovePlayerWalk?.Invoke(sender, move);
        }

        public delegate void CameraFollowHandler(in PlayerView sender);
        public static event CameraFollowHandler OnCameraFollow;
        public static void InvokeOnCameraFollow(in PlayerView sender)
        {
            OnCameraFollow?.Invoke(sender);
        }

        public delegate void SetDesinationHandler(in EnemyData model, in Vector3 pos);
        public static event SetDesinationHandler OnSetDestination;
        public static void InvokeOnSetDestination(in EnemyData model, in Vector3 pos)
        {
            OnSetDestination?.Invoke(model, pos);
        }

        public delegate void ChasePlayerHandler(in EnemyData model, in Vector3 pos);
        public static event ChasePlayerHandler OnChasePlayer;
        public static void InvokeOnChasePlayer(in EnemyData model, in Vector3 pos)
        {
            OnChasePlayer?.Invoke(model, pos);
        }

        public delegate void ChasePlayerEndHandler(in EnemyData model, in Vector3 pos);
        public static event ChasePlayerEndHandler OnChasePlayerEnd;
        public static void InvokeOnChasePlayerEnd(in EnemyData model, in Vector3 pos)
        {
            OnChasePlayerEnd?.Invoke(model, pos);
        }

        public delegate void AttackHandler(in EnemyData model, PlayerView playerView);
        public static event AttackHandler OnAttack;
        public static void InvokeOnAttack(in EnemyData model, PlayerView playerView)
        {
            OnAttack?.Invoke(model, playerView);
        }

        public delegate void PlayerDamagedHandler(in int damage);
        public static event PlayerDamagedHandler OnPlayerDamaged;
        public static void InvokeOnPlayerDamaged(in int damage)
        {
            OnPlayerDamaged?.Invoke(damage);

        }

        public delegate void EnemyDamagedHandler(in int damage, in EnemyData model);
        public static event EnemyDamagedHandler OnEnemyDamaged;
        public static void InvokeOnEnemyDamage(in int damage, in EnemyData model)
        {
            OnEnemyDamaged?.Invoke(damage, model);
        }

        public delegate void ValueChangedHandler(in object sender, in float value, in float maxValue);
        public static event ValueChangedHandler OnValueChanged;
        public static void InvokeOnValueChanged(in object sender, in float value, in float maxValue)
        {
            OnValueChanged?.Invoke(sender, value, maxValue);
        }

        public delegate void ProjectileMoveHandler(in ProjectileData model);
        public static event ProjectileMoveHandler OnProjectileMove;
        public static void InvokeOnProjectileMove(in ProjectileData model)
        {
            OnProjectileMove?.Invoke(model);
        }

        public delegate void HealthChangedHandler(in IdData idSender, in IdData idReceiver);
        public static event HealthChangedHandler OnHealthChanged;
        public static void InvokeOnHealthChanged(in IdData idSender, in IdData idReceiver)
        {
            OnHealthChanged?.Invoke(idSender, idReceiver);
        }

        public delegate void CollisionEnterHandler(in Collider first, in Collider second);
        public static event CollisionEnterHandler OnCollisionEnter;
        public static void InvokeOnCollisionEnter(in Collider first, in Collider second)
        {
            OnCollisionEnter?.Invoke(first, second);
        }

        public delegate void DeathHandler(in object sender);
        public static event DeathHandler OnDeath;
        public static void InvokeOnDeath(in object sender)
        {
            OnDeath?.Invoke(sender);
        }

        public delegate void InitHandler(in object sender);
        public static event InitHandler OnInit;
        public static void InvokeOnInit(in object sender)
        {
            OnInit?.Invoke(sender);
        }

        public delegate void ShootHandler(in object sender);
        public static event ShootHandler OnShoot;
        public static void InvokeOnShoot(in object sender)
        {
            OnShoot?.Invoke(sender);
        }
        ///////////////////////////////////////////
    }
}
