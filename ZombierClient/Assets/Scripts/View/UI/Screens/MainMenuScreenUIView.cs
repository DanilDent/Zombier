using DG.Tweening;
using Prototype.Data;
using Prototype.Service;
using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Prototype.View
{
    public class MainMenuScreenUIView : ScreenUIViewBase
    {

        // TODO: remove appData from here, use actual user locations data
        [Inject]
        public void Construct(AppEventService appEventService, AppData appData)
        {
            _appEventService = appEventService;
            _appData = appData;
        }

        // Injected
        private AppEventService _appEventService;
        private AppData _appData;
        // From inspector
        [SerializeField] private Button _btnPlay;

        [SerializeField] private RectTransform _resumeSessionPopupRect;
        [SerializeField] private Button _btnYesResume;
        [SerializeField] private Button _btnNoResume;
        //

        private void OnEnable()
        {
            _btnPlay.onClick.AddListener(OnPlay);
            _btnYesResume.onClick.AddListener(OnYesResume);
            _btnNoResume.onClick.AddListener(OnNoResume);
            //
            _appEventService.UserHasUnfinishedGameSession += HandleUserHasUnfinishedGameSession;

            if (_appData.User.GameSession != null)
            {
                _appEventService.OnUserHasUnfinishedGameSession();
            }
        }

        private void OnDisable()
        {
            _btnPlay.onClick.RemoveAllListeners();
            _btnYesResume.onClick.RemoveAllListeners();
            _btnNoResume.onClick.RemoveAllListeners();
            //
            _appEventService.UserHasUnfinishedGameSession -= HandleUserHasUnfinishedGameSession;
        }

        private void OnPlay()
        {
            _appEventService.OnPlay(new PlayEventArgs { LocationId = "Id_Locaiton0" });
        }

        private void OnYesResume()
        {
            _appEventService.OnResumeGameSession();
            _resumeSessionPopupRect.gameObject.SetActive(false);
        }

        private void OnNoResume()
        {
            _appEventService.OnResetGameSession();
            _resumeSessionPopupRect.gameObject.SetActive(false);
        }

        private void HandleUserHasUnfinishedGameSession(object sender, EventArgs e)
        {
            _resumeSessionPopupRect.gameObject.SetActive(true);
            _resumeSessionPopupRect.DOScale(0.1f, duration: 0.5f).From();
        }
    }
}
