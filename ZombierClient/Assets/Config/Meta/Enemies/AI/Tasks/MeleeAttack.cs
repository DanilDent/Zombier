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

        //Use for initialization. This is called only once in the lifetime of the task.
        //Return null if init was successfull. Return an error string otherwise
        protected override string OnInit()
        {
            return null;
        }

        //This is called once each time the task is enabled.
        //Call EndAction() to mark the action as finished, either in success or failure.
        //EndAction can be called from anywhere.
        protected override void OnExecute()
        {
            Subscribe();
        }

        //Called once per frame while the action is active.
        protected override void OnUpdate()
        {
            Update();
        }

        //Called when the task is disabled.
        protected override void OnStop()
        {
            Unsubscribe();
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

        private void Update()
        {
            EventService.value.OnEnemyAttack(new GameEventService.EnemyAttackEventArgs { EntityId = agent.Id });
            EndAction(true);
        }

        private void HandleAttackAnimationEvent(object sender, GameEventService.EnemyAttackAnimationEventArgs e)
        {
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