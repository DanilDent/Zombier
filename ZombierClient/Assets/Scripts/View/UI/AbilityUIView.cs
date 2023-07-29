using Prototype.Data;
using Prototype.Service;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.View
{
    public class AbilityUIView : MonoBehaviour
    {

        public void Init(BuffConfig config, GameEventService eventService, AppEventService appEventService)
        {
            _config = config;
            _eventService = eventService;
            _appEventService = appEventService;

            _nameText.text = config.Name;
            _descriptionText.text = config.DescriptionText;

            _chooseBtn.onClick.RemoveAllListeners();
            _chooseBtn.onClick.AddListener(() =>
            {
                _eventService.OnPlayerBuffApplied(new GameEventService.PlayerBuffAppliedEventArgs { BuffId = _config.Id });
                _appEventService.OnGameUnpause();
                _eventService.OnChooseBuffWindowClose();
            });
        }

        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Button _chooseBtn;
        //
        private BuffConfig _config;
        private GameEventService _eventService;
        private AppEventService _appEventService;
    }
}
