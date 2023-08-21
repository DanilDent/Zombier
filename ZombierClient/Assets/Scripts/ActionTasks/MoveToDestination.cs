using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Prototype.Model;
using Prototype.Service;
using UnityEngine;

namespace Prototype.ActionTasks
{

    [Category("Prototype/Movement")]
    public class MoveToDestination : ActionTask<EnemyModel>
    {
        // Public

        // From blackboard
        public BBParameter<GameEventService> EventService;

        // Protected

        protected override void OnUpdate()
        {
            UpdateEnemy(agent);
        }

        // Private

        private void UpdateEnemy(EnemyModel enemy)
        {
            SyncAgentWithTransform(enemy);

            HandlePathFindingInput(enemy);
            HandleRotationNoFight(enemy);
            HandleMovement(enemy);

            EventService?.value?.OnEnemyMoved(new GameEventService.EnemyMovedEventArgs { Id = enemy.Id, Value = enemy.CurrentMovement == Vector3.zero ? 0f : 1f });
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
            enemy.transform.position += enemy.CurrentMovement * enemy.MaxSpeed * Time.deltaTime;
        }

        private void HandleRotationNoFight(EnemyModel enemy)
        {
            Vector3 positionToLookAt = new Vector3(enemy.CurrentMovement.x, 0f, enemy.CurrentMovement.z).normalized;

            if (enemy.IsMoving && positionToLookAt != Vector3.zero)
            {
                Quaternion currentRotation = enemy.transform.rotation;
                Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
                enemy.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, enemy.RotationMultiplier * Time.deltaTime);
            }
        }
    }
}