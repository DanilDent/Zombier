using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Prototype.Model;
using Prototype.ObjectPool;
using Prototype.Service;
using UnityEngine;

namespace Prototype.Enemies.AI
{

    [Category("Prototype/Attacks")]
    public class RangeAttack : ActionTask<EnemyModel>
    {
        // From blackboard
        public BBParameter<PlayerModel> Player;
        public BBParameter<GameEventService> EventService;
        public BBParameter<MonoObjectPool<EnemyProjectileModel>> ProjectilePool;

        protected override string OnInit()
        {
            Subscribe();
            return null;
        }

        //This is called once each time the task is enabled.
        //Call EndAction() to mark the action as finished, either in success or failure.
        //EndAction can be called from anywhere.
        protected override void OnExecute()
        {
            agent.Agent.SetDestination(agent.transform.position);
        }

        protected override void OnUpdate()
        {
            Execute();
            EndAction(true);
        }

        protected override void OnPause()
        {
            Unsubscribe();
        }

        // Private

        private void Subscribe()
        {
            EventService.value.EnemyAttackAnimationEvent += HandleAttackAnimationEvent;
        }

        private void Unsubscribe()
        {
            EventService.value.EnemyAttackAnimationEvent -= HandleAttackAnimationEvent;
        }

        private void Execute()
        {
            EventService.value.OnEnemyAttack(new GameEventService.EnemyAttackEventArgs { EntityId = agent.Id });
        }

        private void HandleAttackAnimationEvent(object sender, GameEventService.EnemyAttackAnimationEventArgs e)
        {
            if (agent == null || e == null)
                return;

            if (agent.Id == e.EntityId)
            {
                Shoot();
            }
        }

        private void Shoot()
        {
            float playerHeight = 1f;
            Vector3 targetPosition = Player.value.transform.position + Vector3.up * (playerHeight * .5f);
            Vector3 shootDir = (targetPosition - agent.ShootingPoint.position).normalized;
            Quaternion rot = Quaternion.LookRotation(shootDir);

            EnemyProjectileModel projectile = ProjectilePool.value.Create(agent.ShootingPoint.position, rot);
            projectile.Sender = agent;

            float randomThrustMultiplier = 1.5f;
            projectile.Rigidbody.AddForce(shootDir * Random.Range(agent.Thrust, agent.Thrust * randomThrustMultiplier), ForceMode.Impulse);
            float torqueMultiplier = 1000f;
            projectile.Rigidbody.AddTorque(Random.insideUnitSphere * torqueMultiplier);
        }
    }
}