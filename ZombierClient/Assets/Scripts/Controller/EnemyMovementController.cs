using Prototype.Model;
using Prototype.Service;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    public class EnemyMovementController : MonoBehaviour
    {
        // Public
        [Inject]
        public void Construct(GameEventService eventService, List<EnemyModel> enemies, PlayerModel player)
        {
            _eventService = eventService;
            _enemies = enemies;
            _player = player;
        }

        // Private

        // Injected
        private GameEventService _eventService;
        private List<EnemyModel> _enemies;
        private PlayerModel _player;
        //
        [SerializeField] private float _obstacleAvoidanceRadius = 1f;

        private void OnEnable()
        {
            foreach (var enemy in _enemies)
            {
                enemy.Agent.updatePosition = false;
                enemy.Agent.updateRotation = false;
                enemy.Agent.updateUpAxis = false;

                enemy.MovingForce = enemy.Acceleration * enemy.Rigidbody.mass;
                enemy.StoppingForce = enemy.Deceleration * enemy.Rigidbody.mass;
            }
        }

        private void FixedUpdate()
        {
            foreach (var enemy in _enemies)
            {
                enemy.Agent.stoppingDistance = enemy.AttackRange;
                enemy.Agent.radius = _obstacleAvoidanceRadius;
                UpdateEnemy(enemy);
            }
        }

        private void UpdateEnemy(EnemyModel enemy)
        {
            SyncAgentWithTransform(enemy);

            switch (enemy.CurrentState)
            {
                case EnemyModel.State.Idle:
                    // Enemy is waiting
                    break;
                case EnemyModel.State.Chase:
                    // Enemy is chasing
                    HandlePathFindingInput(enemy);
                    HandleRotationNoFight(enemy);
                    HandleMovement(enemy);
                    break;
                case EnemyModel.State.Attack:
                    // Enemy is attacking
                    HandlePathFindingInput(enemy);
                    HandleRotationFight(enemy);
                    HandleMovement(enemy);
                    break;
                case EnemyModel.State.Dead:
                    // Enemy is dead
                    break;
                default:
                    throw new NotImplementedException($"Enemy {enemy} is in unknown state {enemy.CurrentState}");
            }

            _eventService.OnEnemyMoved(new GameEventService.EnemyMovedEventArgs { Id = enemy.Id, Value = enemy.CurrentSpeed / enemy.MaxSpeed });
        }

        private void SyncAgentWithTransform(EnemyModel enemy)
        {
            float epsilon = 1e-2f;
            if (Vector3.Distance(enemy.Agent.nextPosition, enemy.transform.position) > epsilon)
            {
                enemy.Agent.nextPosition = enemy.transform.position;
            }
        }

        private void HandlePathFindingInput(EnemyModel enemy)
        {
            enemy.CurrentMovement = new Vector3(enemy.Agent.desiredVelocity.normalized.x, 0f, enemy.Agent.desiredVelocity.normalized.z);

            if (Vector3.Distance(enemy.transform.position, enemy.Agent.destination) < enemy.Agent.stoppingDistance)
            {
                enemy.CurrentMovement = Vector3.zero;
            }

            //Debug.DrawRay(enemy.transform.position + Vector3.up * 0.5f, enemy.CurrentMovement * 10f, Color.cyan, 1f);
        }

        private void HandleMovement(EnemyModel enemy)
        {
            if (enemy.IsMoving)
            {
                float movingForce = enemy.MovingForce;

                Vector3 localVelocity = enemy.transform.InverseTransformDirection(enemy.Rigidbody.velocity);
                float totalAccelerationTillMax = enemy.MaxSpeed - Mathf.Max(0f, localVelocity.z);
                float deltaAccelerationMax = (enemy.MovingForce / enemy.Rigidbody.mass) * Time.fixedDeltaTime;

                if (deltaAccelerationMax > totalAccelerationTillMax)
                {
                    movingForce = (enemy.Rigidbody.mass * totalAccelerationTillMax) / Time.fixedDeltaTime;
                }

                enemy.Rigidbody.AddForce(enemy.transform.forward * movingForce, ForceMode.Force);
                enemy.CurrentSpeed += (movingForce / enemy.Rigidbody.mass) * Time.fixedDeltaTime;
                enemy.CurrentSpeed = Mathf.Clamp(enemy.CurrentSpeed, 0f, enemy.MaxSpeed);

                Debug.DrawRay(enemy.transform.position + Vector3.up * 0.5f, enemy.Rigidbody.velocity, Color.green, 1f);
            }
            else if (enemy.CurrentSpeed > 0)
            {
                enemy.CurrentSpeed -= (enemy.StoppingForce / enemy.Rigidbody.mass) * Time.fixedDeltaTime;
                enemy.CurrentSpeed = Mathf.Clamp(enemy.CurrentSpeed, 0f, enemy.MaxSpeed);
            }
        }

        private void HandleRotationNoFight(EnemyModel enemy)
        {
            Vector3 positionToLookAt = new Vector3(enemy.CurrentMovement.x, 0f, enemy.CurrentMovement.z).normalized;

            if (enemy.IsMoving && positionToLookAt != Vector3.zero)
            {
                Vector3 velocityLocalZ = new Vector3(0f, 0f, Mathf.Max(0f, enemy.transform.InverseTransformDirection(enemy.Rigidbody.velocity).z));
                Vector3 velocityZ = enemy.transform.TransformDirection(velocityLocalZ);
                Vector3 newVelocityZ = Vector3.Slerp(velocityZ, positionToLookAt, enemy.RotationMultiplier * Time.fixedDeltaTime);
                enemy.Rigidbody.velocity = newVelocityZ + (enemy.Rigidbody.velocity - velocityZ);

                Quaternion currentRotation = enemy.transform.rotation;
                Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
                enemy.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, enemy.RotationMultiplier * Time.fixedDeltaTime);
            }
        }

        private void HandleRotationFight(EnemyModel enemy)
        {
            if (_player == null)
                return;

            Vector3 lookDireciton = _player.transform.position - enemy.transform.position;
            lookDireciton = lookDireciton.normalized;

            Vector3 postitionToLookAt = lookDireciton;
            postitionToLookAt.y = 0;

            Quaternion currentRotation = enemy.transform.rotation;
            Quaternion targetRotation = Quaternion.LookRotation(postitionToLookAt);

            enemy.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, enemy.RotationMultiplier * Time.fixedDeltaTime);
        }

        private void OnDisable()
        {

            foreach (var enemy in _enemies)
            {
                _eventService.OnEnemyMoved(new GameEventService.EnemyMovedEventArgs { Id = enemy.Id, Value = 0f });
                enemy.Rigidbody.velocity = Vector3.zero;
            }
        }
    }

}