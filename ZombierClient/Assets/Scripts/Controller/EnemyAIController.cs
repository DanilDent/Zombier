using Prototype.Data;
using Prototype.Model;
using Prototype.ObjectPool;
using Prototype.Service;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    public class EnemyAIController : MonoBehaviour
    {
        // Public
        // 
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
        //
        private Dictionary<IdData, Coroutine> _restoreFromHitCoroutines;

        private void Awake()
        {
            _restoreFromHitCoroutines = new Dictionary<IdData, Coroutine>();
        }

        private void OnEnable()
        {
            // Events
            _eventService.EnemyHitEnd += HandleEnemyHitEnd;

            foreach (var enemy in _enemies)
            {
                enemy.Agent.updatePosition = false;
                enemy.Agent.updateRotation = false;
                enemy.Agent.updateUpAxis = false;
                enemy.Agent.stoppingDistance = enemy.AttackRange;

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
                Debug.Log($"Current state: {enemy.CurrentState}");
            }
        }

        private void OnDisable()
        {
            _eventService.EnemyHitEnd -= HandleEnemyHitEnd;
        }

        private void HandleEnemyHitEnd(object sender, GameEventService.EnemyHitEventArgs e)
        {
            var enemy = _enemies.FirstOrDefault(_ => _.Id.Equals(e.EntityId));
            if (enemy == null)
            {
                return;
            }
            if (enemy.CurrentState != HumanoidState.Hit)
            {
                return;
            }
            if (_restoreFromHitCoroutines.ContainsKey(enemy.Id))
            {
                StopCoroutine(_restoreFromHitCoroutines[enemy.Id]);
            }
            enemy.CurrentState = HumanoidState.RestoringFromHit;
            float restoreDelaySec = 1f;
            _restoreFromHitCoroutines[enemy.Id] = StartCoroutine(Helpers.InvokeWithDelay(() =>
            {
                if (enemy.CurrentState == HumanoidState.RestoringFromHit)
                {
                    enemy.CurrentState = HumanoidState.Idle;
                }
            }, restoreDelaySec));
        }
    }
}
