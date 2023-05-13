using DG.Tweening;
using Prototype.Data;
using Prototype.Model;
using Prototype.Service;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Prototype.View
{
    public class EnemyHealthBarUIView : MonoBehaviour
    {
        // Public

        [Inject]
        public void Construct(IdData id, GameplayEventService eventService)
        {
            _id = id;
            _eventService = eventService;
        }

        // Private

        // Injected
        private IdData _id;
        private GameplayEventService _eventService;
        // From inspector
        [SerializeField] private Image _imgFiller;
        [SerializeField] private Image _imgFillerFollower;
        [SerializeField] private float _followDuration = 1f;

        private void OnEnable()
        {
            _eventService.Damaged += HandleDamaged;
            _eventService.EnemyDeath += HandleDeath;
        }

        private void OnDisable()
        {
            _eventService.Damaged -= HandleDamaged;
            _eventService.EnemyDeath -= HandleDeath;
        }

        private void HandleDamaged(object sender, GameplayEventService.DamagedEventArgs e)
        {
            if (_id == e.EntityId)
            {
                _imgFiller.fillAmount = e.DamagedEntity.Health / e.DamagedEntity.MaxHealth;
                DOTween.To(() => _imgFillerFollower.fillAmount, x => _imgFillerFollower.fillAmount = x, _imgFiller.fillAmount, _followDuration);
            }
        }

        private void HandleDeath(object sender, GameplayEventService.EnemyDeathEventArgs e)
        {
            if (e.Entity is EnemyModel cast && _id == cast.Id)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
