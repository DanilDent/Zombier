using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Prototype.Model;
using UnityEngine;

namespace Prototype.ActionTasks
{

    [Category("Prototype/Movement")]
    public class RotateToTarget : ActionTask<EnemyModel>
    {
        // Public
        public BBParameter<Transform> Player;

        // Protected

        protected override void OnUpdate()
        {
            Rotate(agent);
            EndAction(true);
        }

        // Private

        private void Rotate(EnemyModel enemy)
        {
            if (Player.value == null)
                return;

            Vector3 lookDireciton = Player.value.position - enemy.transform.position;
            lookDireciton = lookDireciton.normalized;

            Vector3 postitionToLookAt = lookDireciton;
            postitionToLookAt.y = 0;

            Quaternion currentRotation = enemy.transform.rotation;
            Quaternion targetRotation = Quaternion.LookRotation(postitionToLookAt);

            enemy.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, enemy.RotationMultiplier * Time.fixedDeltaTime);
        }
    }
}