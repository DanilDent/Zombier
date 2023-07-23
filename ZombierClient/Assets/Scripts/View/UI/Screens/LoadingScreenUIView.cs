using Prototype.Service;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Prototype.View
{
    public class LoadingScreenUIView : ScreenUIViewBase
    {
        [Inject]
        public void Construct(SceneLoaderService sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public enum LoadingState
        {
            None,
            DownloadingGameBalance,
            LoadingScene
        }

        public void SetLoadingState(LoadingState state)
        {
            _loadingState = state;
            switch (state)
            {
                case LoadingState.None:
                    _displayText = "Please wait";
                    UpdateDefault();
                    break;
                case LoadingState.DownloadingGameBalance:
                    _displayText = "Downloading game data";
                    UpdateWhenDownloadingData();
                    break;
                case LoadingState.LoadingScene:
                    _displayText = "Loading";
                    UpdateWhenLoadingGame();
                    break;
                default:
                    throw new NotImplementedException($"Invalid loading state enum value: {state}");
            }
        }

        // Injected
        private SceneLoaderService _sceneLoader;
        // From inspector
        [SerializeField] private TextMeshProUGUI _textLoading;
        [SerializeField] private Image _imgProgressBar;
        [SerializeField] private TextMeshProUGUI _textProgressBar;
        // Internal variables
        private float _timer;
        private float _timerMax = .5f;
        private int _dotsCount = 1;
        private string _displayText;
        private LoadingState _loadingState;

        private void Start()
        {
            SetLoadingState(LoadingState.LoadingScene);
            UpdateDefault();
        }

        private void Update()
        {
            switch (_loadingState)
            {
                case LoadingState.None:
                    UpdateDefault();
                    break;
                case LoadingState.DownloadingGameBalance:
                    UpdateWhenDownloadingData();
                    break;
                case LoadingState.LoadingScene:
                    UpdateWhenLoadingGame();
                    break;
                default:
                    throw new NotImplementedException($"Invalid loading state enum value: {_loadingState}");
            }
        }

        private void UpdateWhenDownloadingData()
        {
            _imgProgressBar.fillAmount = 0;
            _textProgressBar.text = "";

            _timer -= Time.unscaledDeltaTime;
            if (_timer < 0)
            {
                _timer = _timerMax;

                _dotsCount += 1;
                if (_dotsCount > 3)
                {
                    _dotsCount = 1;
                }
                string loadingText = _displayText;

                for (int i = 0; i < _dotsCount; ++i)
                {
                    loadingText += ".";
                }

                _textLoading.text = loadingText;
            }
        }

        private void UpdateWhenLoadingGame()
        {
            float progress = _sceneLoader.GetLoadingProgress();
            _imgProgressBar.fillAmount = progress;
            _textProgressBar.text = $"{Math.Round(progress, 2) * 100}%";

            _timer -= Time.unscaledDeltaTime;
            if (_timer < 0)
            {
                _timer = _timerMax;

                _dotsCount += 1;
                if (_dotsCount > 3)
                {
                    _dotsCount = 1;
                }
                string loadingText = _displayText;

                for (int i = 0; i < _dotsCount; ++i)
                {
                    loadingText += ".";
                }

                _textLoading.text = loadingText;
            }
        }

        private void UpdateDefault()
        {
            _imgProgressBar.fillAmount = 0;
            _textProgressBar.text = "Please wait";

            _timer -= Time.unscaledDeltaTime;
            if (_timer < 0)
            {
                _timer = _timerMax;

                _dotsCount += 1;
                if (_dotsCount > 3)
                {
                    _dotsCount = 1;
                }
                string loadingText = _displayText;

                for (int i = 0; i < _dotsCount; ++i)
                {
                    loadingText += ".";
                }

                _textLoading.text = loadingText;
            }
        }
    }
}
