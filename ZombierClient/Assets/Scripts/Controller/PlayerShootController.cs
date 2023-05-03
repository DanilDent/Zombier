using Prototype.Model;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    public class PlayerShootController : MonoBehaviour
    {
        // Public

        [Inject]
        public void Construct(PlayerModel player, List<EnemyModel> enemies, ProjectileModel.Factory projectileFactory)
        {
            _player = player;
            _enemies = enemies;
            _projectileFactory = projectileFactory;
        }

        // Private

        // Dependencies

        // Injected
        private PlayerModel _player;
        private List<EnemyModel> _enemies;
        private ProjectileModel.Factory _projectileFactory;
        //
        private float _timer;
        private float _timerMax;

        private void Start()
        {
            int secInMin = 60;
            float rps = (float)_player.WeaponModel.FireRateRPM / secInMin;
            _timerMax = 1f / rps;
        }

        private void FixedUpdate()
        {
            if (_player.CurrentState == PlayerModel.State.Fight)
            {
                _timer -= Time.deltaTime;
                if (_timer < 0)
                {
                    Shoot();
                    _timer = _timerMax;
                }
            }
        }

        private void Shoot()
        {
            WeaponModel weapon = _player.WeaponModel;
            ProjectileModel projectile = _projectileFactory.Create(weapon.ProjectilePrefab);
            projectile.transform.position = weapon.ShootingPoint.position;
            projectile.transform.rotation = weapon.ShootingPoint.rotation;
            projectile.Rigidbody.AddForce(projectile.transform.forward * projectile.Speed, ForceMode.Impulse);
            Debug.Log("Shoot!");
        }
    }
}
