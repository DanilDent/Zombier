using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Prototype.Model;
using Prototype.Service;
using UnityEngine;

namespace Prototype.Enemies.AI
{

    [Category("Prototype/Attacks")]
    public class MeleeAttack : ActionTask<EnemyModel>
    {
        // Public

        // From blackboard
        public BBParameter<GameEventService> EventService;
        public BBParameter<PlayerModel> Player;

        protected override string OnInit()
        {
            Subscribe();
            return null;
        }

        protected override void OnExecute()
        {
            agent.Agent.SetDestination(agent.transform.position);
        }

        //Called once per frame while the action is active.
        protected override void OnUpdate()
        {
            Execute();
            EndAction(true);
        }

        //Called when the task is paused.
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
                    EventService.value.OnAttacked(new GameEventService.AttackedEventArgs { Attacker = agent, Defender = Player.value });
                }
            }
        }
    }
}