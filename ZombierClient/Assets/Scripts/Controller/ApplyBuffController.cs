using Prototype.Model;
using Prototype.Service;
using UnityEngine;
using Zenject;

namespace Prototype
{
    public class ApplyBuffController : MonoBehaviour
    {
        [Inject]
        public void Construct(
            PlayerModel player,
            GameEventService eventService,
            BuffFactory buffFactory)
        {
            _player = player;
            _eventService = eventService;
            _buffFactory = buffFactory;
        }

        private void OnEnable()
        {
            _eventService.PlayerBuffApplied += HandleApplyBuff;
        }

        private void Start()
        {
            ApplyAllBuffs();
        }

        private void OnDisable()
        {
            _eventService.PlayerBuffApplied -= HandleApplyBuff;
        }

        private void OnDestroy()
        {
            CancelAllBuffs();
        }

        private void ApplyAllBuffs()
        {
            foreach (var buffId in _player.AppliedBuffs)
            {
                Buff buff = _buffFactory.Create(buffId);
                buff.Apply(updateSessionData: false);
                Debug.Log($"Saved buff applied: {buff.Config.Id}");
            }
        }

        private void CancelAllBuffs()
        {
            foreach (var buffId in _player.AppliedBuffs)
            {
                Buff buff = _buffFactory.Create(buffId);
                buff.Cancel(updateSessionData: false);
                Debug.Log($"Buff canceled runtime");
            }
        }

        private void HandleApplyBuff(object sender, GameEventService.PlayerBuffAppliedEventArgs e)
        {
            Buff buff = _buffFactory.Create(e.BuffId);
            buff.Apply(_player);
            Debug.Log($"Buff applied: {buff.Config.Id}");
        }

        // Private

        // Dependencies

        // Injected
        private PlayerModel _player;
        private GameEventService _eventService;
        private BuffFactory _buffFactory;
    }
}
