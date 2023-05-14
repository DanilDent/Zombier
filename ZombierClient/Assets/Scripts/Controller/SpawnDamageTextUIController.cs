using Prototype.Data;
using Prototype.ObjectPool;
using Prototype.Service;
using Prototype.View;
using System.Collections.Generic;
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
        private List<DamageTextUIView> _damageTextList;

        private void OnEnable()
        {
            // Events
            _eventService.Damaged += HandleDamaged;
            //
            _damageTextList = new List<DamageTextUIView>();
        }

        private void OnDisable()
        {
            // Events
            _eventService.Damaged -= HandleDamaged;
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
        }

        private void HandleDamaged(object sender, GameplayEventService.DamagedEventArgs e)
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
    }
}
