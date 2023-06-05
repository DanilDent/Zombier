using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Prototype.Model;
using UnityEngine;

namespace Prototype.Enemies.AI
{
    [Category("Prototype/Movement")]
    public class SetDestination : ActionTask<EnemyModel>
    {
        public BBParameter<Vector3> Destination;

        protected override string info => $"Set destination to {Destination.value}";

        protected override void OnExecute()
        {
            agent.Agent.SetDestination(Destination.value);
        }

        protected override void OnUpdate()
        {
            agent.Agent.SetDestination(Destination.value);
            SyncAgentWithTransform(agent);
        }

        private void SyncAgentWithTransform(EnemyModel enemy)
        {
            float epsilon = 1e-2f;
            if (Vector3.Distance(enemy.Agent.nextPosition, enemy.transform.position) > epsilon)
            {
                enemy.Agent.nextPosition = enemy.transform.position;
            }
        }
    }
}