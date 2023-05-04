using Prototype.Model;
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
            ProjectileModel.Factory projectileFactory,
            MarkerProjectiles markerProjectiles,
            GameplayEventService eventService)
        {
            _player = player;
            _enemies = enemies;
            _projectileFactory = projectileFactory;
            _markerProjectiles = markerProjectiles;
            _eventService = eventService;
        }

        // Private

        // Dependencies

        // Injected
        private PlayerModel _player;
        private List<EnemyModel> _enemies;
        private ProjectileModel.Factory _projectileFactory;
        private MarkerProjectiles _markerProjectiles;
        private GameplayEventService _eventService;
        //
        private float _timer;
        private float _timerMax;

        private void Start()
        {
            int secInMin = 60;
            float rps = (float)_player.WeaponModel.FireRateRPM / secInMin;
            _timerMax = 1f / rps;
        }

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
            WeaponModel weapon = _player.WeaponModel;
            ProjectileModel projectile = _projectileFactory.Create(weapon.ProjectilePrefab);
            projectile.transform.SetParent(_markerProjectiles.transform);
            Vector3 shootDir = (_player.CurrentTarget.TargetPoint.position - weapon.WeaponEndPoint.position).normalized;
            Quaternion rot = Quaternion.LookRotation(shootDir);
            projectile.transform.position = weapon.WeaponEndPoint.position;
            projectile.transform.rotation = rot;
            projectile.Rigidbody.AddForce(shootDir * projectile.Thrust, ForceMode.Impulse);
            float projectileLifeTime = 3f;
            Destroy(projectile.gameObject, projectileLifeTime);
        }
    }
}
