using Prototype.Model;
using Prototype.Service;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    public class EnemyMovementController : MonoBehaviour
    {
        // Public
        [Inject]
        public void Construct(GameplayEventService eventService, List<EnemyModel> enemies)
        {
            _eventService = eventService;
            _enemies = enemies;
        }

        // Private

        // Injected
        private GameplayEventService _eventService;
        private List<EnemyModel> _enemies;
        //

        private void Start()
        {
            foreach (var enemy in _enemies)
            {
                enemy.Agent.updatePosition = false;
                enemy.Agent.updateRotation = false;
                enemy.Agent.updateUpAxis = false;
            }
        }

        private void Update()
        {
            foreach (var enemy in _enemies)
            {
                UpdateEnemy(enemy);
            }
        }

        private void UpdateEnemy(EnemyModel enemy)
        {
            HandlePathFindingInput(enemy);
            HandleGravity(enemy);
            HandleAcceleration(enemy);
            HandleMovement(enemy);
            HandleRotation(enemy);
        }

        private void HandlePathFindingInput(EnemyModel enemy)
        {
            enemy.CurrentMovement = new Vector3(enemy.Agent.desiredVelocity.x, 0f, enemy.Agent.desiredVelocity.z);
            float deadInputZone = 1e-2f;
            if (enemy.CurrentMovement.magnitude < deadInputZone)
            {
                enemy.CurrentMovement = Vector3.zero;
            }
        }

        private void HandleGravity(EnemyModel enemy)
        {
            float gravity = -9.8f;
            enemy.CurrentMovement = new Vector3(enemy.CurrentMovement.x, gravity, enemy.CurrentMovement.z);
        }

        private void HandleAcceleration(EnemyModel enemy)
        {
            if (enemy.IsMoving())
            {
                enemy.CurrentSpeed += enemy.Acceleration * Time.deltaTime;
            }
            else if (enemy.CurrentSpeed > 0)
            {
                enemy.CurrentSpeed -= enemy.Deceleration * Time.deltaTime;
            }

            enemy.CurrentSpeed = Mathf.Clamp(enemy.CurrentSpeed, 0f, enemy.Speed);
        }

        private void HandleMovement(EnemyModel enemy)
        {
            enemy.CharacterController.Move(enemy.CurrentMovement * enemy.CurrentSpeed * Time.deltaTime);

            float epsilon = 1e-2f;
            if (Vector3.Distance(enemy.Agent.nextPosition, enemy.transform.position) > epsilon)
            {
                enemy.Agent.nextPosition = enemy.transform.position;
            }

            _eventService.OnEnemyMoved(new GameplayEventService.EnemyMovedEventArgs { Id = enemy.Id, Value = enemy.CurrentSpeed / enemy.Speed });
        }

        private void HandleRotation(EnemyModel enemy)
        {
            Vector3 positionToLookAt = (enemy.Agent.destination - enemy.transform.position).normalized;
            positionToLookAt.y = 0;

            if (positionToLookAt != Vector3.zero)
            {
                Quaternion currentRotation = enemy.transform.rotation;
                Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
                enemy.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, enemy.RotationMultiplier * Time.deltaTime);
            }
        }
    }

}