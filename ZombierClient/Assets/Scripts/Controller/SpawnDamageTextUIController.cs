using Prototype.Data;
using Prototype.Model;
using Prototype.ObjectPool;
using Prototype.Service;
using Prototype.View;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    public class SpawnDamageTextUIController : MonoBehaviour
    {
        // Public

        [Inject]
        public void Construct(GameplayEventService eventService, MonoObjectPool<DamageTextUIView> damageTextPool)
        {
            _eventService = eventService;
            _damageTextPool = damageTextPool;
        }

        // Private

        // Injected
        private GameplayEventService _eventService;
        private MonoObjectPool<DamageTextUIView> _damageTextPool;
        //
        [SerializeField] private DescRandomRange _displayDuration;

        private void OnEnable()
        {
            _eventService.Damaged += HandleDamaged;
        }

        private void OnDisable()
        {
            _eventService.Damaged -= HandleDamaged;
        }

        private void HandleDamaged(object sender, GameplayEventService.DamagedEventArgs e)
        {
            if (e.DamagedEntity is EnemyModel cast)
            {
                Vector3 offset = new Vector3(Random.Range(-.2f, .2f), 1.5f + Random.Range(-.2f, .2f), -.5f);
                Vector3 spawnPosition = cast.transform.position + offset;
                DamageTextUIView instance = _damageTextPool.Create(spawnPosition, Quaternion.identity);
                instance.SetTextValue(e.DamageValue);
                instance.DisplayDuration = Random.Range(_displayDuration.Min, _displayDuration.Max);
                StartCoroutine(_damageTextPool.Destroy(instance, instance.DisplayDuration));
            }
        }
    }
}
