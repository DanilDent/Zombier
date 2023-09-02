using Prototype.Model;
using Prototype.ObjectPool;
using Prototype.Service;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    public class ShootingControllerPlain : ShootingControllerBase
    {
        // Public

        [Inject]
        public void Construct(
            PlayerModel player,
            List<EnemyModel> enemies,
            GameEventService eventService)
        {
            _player = player;
            _enemies = enemies;
            _eventService = eventService;
        }

        // Private

        // Dependencies

        // Injected
        private PlayerModel _player;
        private List<EnemyModel> _enemies;
        [Inject(Id = "DefaultPlayerProjectileObjectPool")] private IMonoObjectPool<PlayerProjectileModel> _defaultProjectilePool;
        [Inject(Id = "BouncePlayerProjectileObjectPool")] private IMonoObjectPool<PlayerProjectileModel> _bounceProjectilePool;
        private GameEventService _eventService;
        //
        private float _timer;
        private float _timerMax;
        private IMonoObjectPool<PlayerProjectileModel> _projectilePool;

        private void OnEnable()
        {
            _eventService.PlayerShootAnimationEvent += Shoot;
            _eventService.BounceProjectilesEnabled += HandleBounceProjectilesEnabled;
        }

        private void Start()
        {
            _projectilePool = _defaultProjectilePool;
        }

        private void OnDisable()
        {
            _eventService.PlayerShootAnimationEvent -= Shoot;
            _eventService.BounceProjectilesEnabled -= HandleBounceProjectilesEnabled;
        }

        private void Update()
        {
            int secInMin = 60;
            float rps = (float)_player.Weapon.FireRateRPM / secInMin;
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

        private void HandleBounceProjectilesEnabled(object sender, EventArgs e)
        {
            _projectilePool = _bounceProjectilePool;
        }

        private void Shoot(object sender, EventArgs e)
        {
            if (_player.CurrentTarget != null)
            {
                WeaponModel weapon = _player.Weapon;
                Vector3 shootDir = weapon.ShootingPoint.forward;

                float recoilX = UnityEngine.Random.Range(-weapon.Recoil, weapon.Recoil);
                float recoilY = UnityEngine.Random.Range(-weapon.Recoil, weapon.Recoil);
                shootDir = Quaternion.Euler(recoilX, recoilY, 0) * shootDir;

                Quaternion rot = Quaternion.LookRotation(shootDir);

                PlayerProjectileModel projectile = _projectilePool.Create(weapon.ShootingPoint.position, rot);
                projectile.Sender = _player;

                projectile.Rigidbody.AddForce(projectile.transform.forward * weapon.Thrust, ForceMode.Impulse);

                float projectileLifeTime = 7f;
                StartCoroutine(_projectilePool.Destroy(projectile, projectileLifeTime));
            }
        }
    }
}
