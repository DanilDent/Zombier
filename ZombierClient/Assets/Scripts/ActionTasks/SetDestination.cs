using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Prototype.Model;
using UnityEngine;

namespace Prototype.ActionTasks
{
    [Category("Prototype/Movement")]
    public class SetDestination : ActionTask<EnemyModel>
    {
        // Public

        // From blackboard
        public BBParameter<Vector3> Destination;

        // Protected

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

        // Private

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