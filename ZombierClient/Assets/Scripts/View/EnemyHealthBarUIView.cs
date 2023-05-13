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
        public void Construct(IdData id, GameplayEventService eventService, Image image)
        {
            _id = id;
            _eventService = eventService;
            _image = image;
        }

        // Private

        // Injected
        private IdData _id;
        private GameplayEventService _eventService;
        private Image _image;

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
                _image.fillAmount = e.DamagedEntity.Health / e.DamagedEntity.MaxHealth;
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
