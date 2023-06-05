using NodeCanvas.Framework;
using ParadoxNotion.Design;
using ParadoxNotion.Services;
using Prototype.Model;
using Prototype.Service;
using UnityEngine;

namespace Prototype.Enemies.AI
{

    [Category("Prototype/Movement")]
    public class MoveToDestinationByRigdidBody : ActionTask<EnemyModel>
    {
        // Public

        // From blackboard
        public BBParameter<GameEventService> EventService;

        protected override void OnExecute()
        {
            MonoManager.current.onFixedUpdate += OnFixedUpdate;
        }

        protected void OnFixedUpdate()
        {
            UpdateEnemy(agent);
        }

        protected override void OnStop()
        {
            MonoManager.current.onFixedUpdate -= OnFixedUpdate;
        }

        // Private

        private void UpdateEnemy(EnemyModel enemy)
        {
            SyncAgentWithTransform(enemy);

            HandlePathFindingInput(enemy);
            HandleRotationNoFight(enemy);
            HandleMovement(enemy);

            EventService.value.OnEnemyMoved(new GameEventService.EnemyMovedEventArgs { Id = enemy.Id, Value = enemy.CurrentSpeed / enemy.MaxSpeed });

            Debug.DrawRay(enemy.Agent.destination, Vector3.up * 2f, Color.red, Time.fixedDeltaTime);
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

            Debug.DrawRay(enemy.transform.position + Vector3.up * 0.5f, enemy.CurrentMovement * 10f, Color.cyan, 1f);
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
    }
}