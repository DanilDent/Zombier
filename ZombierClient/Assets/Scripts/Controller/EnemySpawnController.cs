using Prototype.Model;
using Prototype.SO;
using UnityEngine;
using UnityEngine.AI;

namespace Prototype.Controller
{
    public class EnemySpawnController
    {
        private LevelModel _level;
        private EnemyModel.Factory _enemyFactory;
        private PlayerModel _player;

        public EnemySpawnController(LevelModel level, EnemyModel.Factory enemyFactory, PlayerModel player)
        {
            _level = level;
            _enemyFactory = enemyFactory;
            _player = player;
        }

        private struct SpawnBounds
        {
            public int XMin;
            public int XMax;
            public int ZMin;
            public int ZMax;
        }

        [SerializeField] private float _minDistanceFromPlayer = 10f;
        private readonly SpawnBounds _spawnBounds = new SpawnBounds { XMin = -10, XMax = 10, ZMin = -10, ZMax = 10 };
        private readonly int _spawnableLayer = LayerMask.GetMask("Ground");

        public void SpawnEnemies()
        {
            int enemyCount = Random.Range(_level.EnemySpawnSO.MinEnemyCount, _level.EnemySpawnSO.MaxEnemyCount + 1);
            for (int i = 0; i < enemyCount; ++i)
            {
                EnemySO enemySO = _level.EnemySpawnSO.Enemies[i];
                EnemyModel enemy = _enemyFactory.Create(enemySO);
                enemy.transform.position = GetNewRandomSpawnPositionNavmesh(enemy);
            }
        }

        private Vector3 GetNewRandomSpawnPosition(EnemyModel enemyPfab)
        {
            int maxIterations = 10000;
            while (maxIterations > 0)
            {
                --maxIterations;

                float tryPosX = Random.Range(_spawnBounds.XMin, _spawnBounds.XMax);
                float tryPosY = 50f;
                float tryPosZ = Random.Range(_spawnBounds.ZMin, _spawnBounds.ZMax);

                Vector3 tryPos = new Vector3(tryPosX, tryPosY, tryPosZ);

                var enemyCollider = enemyPfab.GetComponent<CapsuleCollider>();

                Vector3 point1 = tryPos;
                Vector3 point2 = tryPos + Vector3.up * enemyCollider.height;
                float radius = enemyCollider.radius;
                Vector3 down = Vector3.down;
                RaycastHit hit;
                float maxDistance = 10000f;

                if (Physics.CapsuleCast(point1, point2, radius, down, out hit, maxDistance))
                {
                    if (true/*hit.transform.gameObject.layer == _spawnableLayer*/)
                    {
                        Vector3 result = new Vector3(tryPosX, 0f, tryPosZ);
                        if (Vector3.Distance(_player.transform.position, result) > _minDistanceFromPlayer)
                        {
                            return result;
                        }
                    }
                }
            }

            Debug.LogError("Getting random enemy spawn position is taking too long");
            return Vector3.zero;
        }

        private Vector3 GetNewRandomSpawnPositionNavmesh(EnemyModel enemyPfab)
        {
            int maxIterations = 10000;
            while (maxIterations > 0)
            {
                --maxIterations;

                float tryPosX = Random.Range(_spawnBounds.XMin, _spawnBounds.XMax);
                float tryPosY = 50f;
                float tryPosZ = Random.Range(_spawnBounds.ZMin, _spawnBounds.ZMax);

                Vector3 tryPos = new Vector3(tryPosX, 0f, tryPosZ);

                if (NavMesh.SamplePosition(tryPos, out var hit, Mathf.Infinity, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }

            Debug.LogError("Getting random enemy spawn position is taking too long");
            return Vector3.zero;
        }

        //public CapsuleCollider CapsuleColliderRef;
        //private void OnDrawGizmos()
        //{
        //    Vector3 startCapsulePos = transform.position + new Vector3(0f, CapsuleColliderRef.height / 2f) + CapsuleColliderRef.center; /*this line gives you the capsule start position*/
        //    Vector3 finalCapsulePos = transform.position - new Vector3(0f, CapsuleColliderRef.height / 2f) + CapsuleColliderRef.center; /*this line gives you the capsule end position*/
        //    DebugExtension.DrawCapsule(
        //        startCapsulePos,
        //        finalCapsulePos,
        //        Color.blue,
        //        CapsuleColliderRef.radius /* here you have access to collider radius. */
        //        );
        //}
    }

}