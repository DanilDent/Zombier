using UnityEngine;
using UnityEngine.AI;

namespace Prototype.Test
{
    public class TestNavmeshAgent : MonoBehaviour
    {
        public Transform Player;
        public NavMeshAgent Agent;

        private void Update()
        {
            Agent.SetDestination(Player.transform.position);
        }
    }

}