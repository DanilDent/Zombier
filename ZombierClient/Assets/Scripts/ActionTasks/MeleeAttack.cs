using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Prototype.Model;
using Prototype.Service;
using UnityEngine;

namespace Prototype.ActionTasks
{

    [Category("Prototype/Attacks")]
    public class MeleeAttack : ActionTask<EnemyModel>
    {
        // Public

        // From blackboard
        public BBParameter<GameEventService> EventService;
        public BBParameter<PlayerModel> Player;

        // Protected

        protected override string OnInit()
        {
            Subscribe();
            return null;
        }

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
                if (Vector3.Distance(agent.transform.position, Player.value.transform.position) < agent.AttackRange)
                {
                    EventService.value.OnDamageDealt(new GameEventService.DamageDealtEventArgs { Attacker = agent, Defender = Player.value });
                }
            }
        }
    }
}