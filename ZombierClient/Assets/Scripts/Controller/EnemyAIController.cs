﻿using Prototype.Model;
using Prototype.ObjectPool;
using Prototype.Service;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    public class EnemyAIController : MonoBehaviour
    {
        // Public 

        [Inject]
        public void Construct(
            GameEventService eventService,
            List<EnemyModel> enemies,
            PlayerModel player,
            MonoObjectPool<EnemyProjectileModel> projectilePool)
        {
            _eventService = eventService;
            _enemies = enemies;
            _player = player;
            _projectilePool = projectilePool;
        }

        // Private

        // Injected
        private GameEventService _eventService;
        private List<EnemyModel> _enemies;
        private PlayerModel _player;
        private MonoObjectPool<EnemyProjectileModel> _projectilePool;
        // From inspector
        [SerializeField] private float _obstacleAvoidanceRadius = 1f;

        private void OnEnable()
        {
            foreach (var enemy in _enemies)
            {
                enemy.Agent.updatePosition = false;
                enemy.Agent.updateRotation = false;
                enemy.Agent.updateUpAxis = false;
                enemy.Agent.stoppingDistance = enemy.AttackRange;

                enemy.MovingForce = enemy.Acceleration * enemy.Rigidbody.mass;
                enemy.StoppingForce = enemy.Deceleration * enemy.Rigidbody.mass;

                enemy.Blackboard.SetVariableValue("EventService", _eventService);
                enemy.Blackboard.SetVariableValue("Player", _player);
                enemy.Blackboard.SetVariableValue("ProjectilePool", _projectilePool);
            }
        }

        private void Update()
        {
            foreach (var enemy in _enemies)
            {
                enemy.Agent.stoppingDistance = enemy.AttackRange;
                enemy.Agent.radius = _obstacleAvoidanceRadius;
                enemy.FSMOwner.UpdateBehaviour();
            }
        }
    }
}
