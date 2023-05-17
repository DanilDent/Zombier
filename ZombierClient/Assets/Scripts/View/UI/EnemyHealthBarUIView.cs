using DG.Tweening;
using Prototype.Data;
using Prototype.Model;
using Prototype.Service;
using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Prototype.View
{
    public class EnemyHealthBarUIView : MonoBehaviour
    {
        // Public

        [Inject]
        public void Construct(IdData id, GameEventService eventService)
        {
            _id = id;
            _eventService = eventService;
        }

        // Private

        // Injected
        private IdData _id;
        private GameEventService _eventService;
        // From inspector
        [SerializeField] private Image _imgFiller;
        [SerializeField] private Image _imgFillerFollower;
        [SerializeField] private float _followDuration = 1f;

        private void OnEnable()
        {
            _eventService.Damaged += HandleDamaged;
            _eventService.EnemyDeath += HandleEnemyDeath;
            _eventService.PlayerDeath += HandlePlayerDeath;
        }

        private void OnDisable()
        {
            _eventService.Damaged -= HandleDamaged;
            _eventService.EnemyDeath -= HandleEnemyDeath;
            _eventService.PlayerDeath -= HandlePlayerDeath;
        }

        private void HandleDamaged(object sender, GameEventService.DamagedEventArgs e)
        {
            if (_id == e.EntityId)
            {
                _imgFiller.fillAmount = e.DamagedEntity.Health / e.DamagedEntity.MaxHealth;
                DOTween.To(() => _imgFillerFollower.fillAmount, x => _imgFillerFollower.fillAmount = x, _imgFiller.fillAmount, _followDuration);
            }
        }

        private void HandleEnemyDeath(object sender, GameEventService.EnemyDeathEventArgs e)
        {
            if (e.Entity is EnemyModel cast && _id == cast.Id)
            {
                gameObject.SetActive(false);
            }
        }

        private void HandlePlayerDeath(object sender, EventArgs e)
        {
            gameObject.SetActive(false);
        }
    }
}
