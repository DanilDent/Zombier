using Prototype.Model;
using Prototype.ObjectPool;
using Prototype.Service;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    public class PlayerShootController : MonoBehaviour
    {
        // Public

        [Inject]
        public void Construct(
            PlayerModel player,
            List<EnemyModel> enemies,
            MonoObjectPool<ProjectileModel> projectilePool,
            GameplayEventService eventService)
        {
            _player = player;
            _enemies = enemies;
            _projectilePool = projectilePool;
            _eventService = eventService;
        }

        // Private

        // Dependencies

        // Injected
        private PlayerModel _player;
        private List<EnemyModel> _enemies;
        private MonoObjectPool<ProjectileModel> _projectilePool;
        private GameplayEventService _eventService;
        //
        private float _timer;
        private float _timerMax;

        private void OnEnable()
        {
            _eventService.PlayerShootAnimationEvent += Shoot;
        }

        private void OnDisable()
        {
            _eventService.PlayerShootAnimationEvent -= Shoot;
        }

        private void Update()
        {
            int secInMin = 60;
            float rps = (float)_player.WeaponModel.FireRateRPM / secInMin;
            _timerMax = 1f / rps;

            if (_player.CurrentState == PlayerModel.State.Fight)
            {
                _timer -= Time.deltaTime;
                if (_timer < 0)
                {
                    _eventService.OnPlayerShoot();
                    _timer = _timerMax;
                }
            }
            else
            {
                _timer = 0;
            }
        }

        private void Shoot(object sender, EventArgs e)
        {
            if (_player.CurrentTarget != null)
            {
                WeaponModel weapon = _player.WeaponModel;
                Vector3 shootDir = (_player.CurrentTarget.TargetPoint.position - weapon.WeaponEndPoint.position).normalized;

                float recoilX = UnityEngine.Random.Range(-weapon.Recoil, weapon.Recoil);
                float recoilY = UnityEngine.Random.Range(-weapon.Recoil, weapon.Recoil);
                shootDir = Quaternion.Euler(recoilX, recoilY, 0) * shootDir;

                Quaternion rot = Quaternion.LookRotation(shootDir);

                ProjectileModel projectile = _projectilePool.Create(weapon.ProjectilePrefab, weapon.WeaponEndPoint.position, rot);
                projectile.Sender = _player;

                projectile.Rigidbody.AddForce(shootDir * weapon.Thrust, ForceMode.Impulse);

                float projectileLifeTime = 3f;
                StartCoroutine(_projectilePool.Destroy(projectile, projectileLifeTime));
            }
        }
    }
}
