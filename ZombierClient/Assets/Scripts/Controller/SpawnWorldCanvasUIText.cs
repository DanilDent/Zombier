using Prototype.Data;
using Prototype.Model;
using Prototype.ObjectPool;
using Prototype.Service;
using Prototype.View;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Prototype.Controller
{
    public class SpawnWorldCanvasUIText : MonoBehaviour
    {
        // Public

        [Inject]
        public void Construct(
            GameEventService eventService,
            MonoObjectPool<DamageTextUIView> damageTextPool,
            MonoObjectPool<ExpTextUIView> expTextPool)
        {
            _eventService = eventService;
            _damageTextPool = damageTextPool;
            _expTextPool = expTextPool;
        }

        // Private

        // Injected
        private GameEventService _eventService;
        private MonoObjectPool<DamageTextUIView> _damageTextPool;
        private MonoObjectPool<ExpTextUIView> _expTextPool;
        //
        [SerializeField] private DescRandomRange _displayDuration;
        private List<DamageTextUIView> _damageTextList;
        private List<ExpTextUIView> _expTextList;

        private void OnEnable()
        {
            // Events
            _eventService.Damaged += HandleDamaged;
            _eventService.EnemyDeath += HandleEnemyDeath;
            _eventService.EnemyDeathInstant += HandleEnemyDeath;
            //
            _damageTextList = new List<DamageTextUIView>();
            _expTextList = new List<ExpTextUIView>();
        }

        private void OnDisable()
        {
            // Events
            _eventService.Damaged -= HandleDamaged;
            _eventService.EnemyDeath -= HandleEnemyDeath;
            _eventService.EnemyDeathInstant -= HandleEnemyDeath;
            //
            StopAllCoroutines();

            for (int i = 0; i < _damageTextList.Count; ++i)
            {
                DamageTextUIView dmgTxt = _damageTextList[i];
                _damageTextList.RemoveAt(i);
                _damageTextPool.Destroy(dmgTxt);
                --i;
            }
            _damageTextList = null;

            for (int i = 0; i < _expTextList.Count; ++i)
            {
                ExpTextUIView expTxt = _expTextList[i];
                _expTextList.RemoveAt(i);
                _expTextPool.Destroy(expTxt);
                --i;
            }
        }

        private void HandleDamaged(object sender, GameEventService.DamagedEventArgs e)
        {
            if (e.DamagedEntity is Component cast && e.DamageValue > 0)
            {
                Vector3 offset = new Vector3(Random.Range(-.2f, .2f), 1.5f + Random.Range(-.2f, .2f), -.5f);
                Vector3 spawnPosition = cast.transform.position + offset;
                DamageTextUIView instance = _damageTextPool.Create(spawnPosition, Quaternion.identity, enabled: false);
                instance.SetTextValue(e.DamageValue);
                instance.IsCrit = e.IsCrit;
                instance.DisplayDuration = Random.Range(_displayDuration.Min, _displayDuration.Max);
                instance.gameObject.SetActive(true);
                _damageTextList.Add(instance);
                StartCoroutine(_damageTextPool.Destroy(instance, instance.DisplayDuration, () => _damageTextList.Remove(instance)));
            }
        }

        private void HandleEnemyDeath(object sender, GameEventService.EnemyDeathEventArgs e)
        {
            if (e.Entity is EnemyModel enemy)
            {
                Vector3 offset = new Vector3(Random.Range(-.2f, .2f), 2f + Random.Range(-.2f, .2f), -.5f);
                Vector3 spawnPosition = enemy.transform.position + offset;
                ExpTextUIView instance = _expTextPool.Create(spawnPosition, Quaternion.identity, enabled: false); ;
                instance.SetTextValue(enemy.ExpReward);
                float displayDuration = 1.5f;
                instance.DisplayDuration = displayDuration;
                instance.gameObject.SetActive(true);
                _expTextList.Add(instance);
                StartCoroutine(_expTextPool.Destroy(instance, instance.DisplayDuration, () => _expTextList.Remove(instance)));
            }
        }
    }
}
