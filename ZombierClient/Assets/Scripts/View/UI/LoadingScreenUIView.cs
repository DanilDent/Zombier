using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.View
{
    public class LoadingScreenUIView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _textLoading;
        [SerializeField] private Image _imgProgressBar;
        [SerializeField] private TextMeshProUGUI _textProgressBar;
        private float _timer;
        private float _timerMax = .5f;
        private int _dotsCount = 1;


        private void Update()
        {
            float progress = SceneLoaderService.GetLoadingProgress();
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
                string loadingText = "LOADING";
                for (int i = 0; i < _dotsCount; ++i)
                {
                    loadingText += ".";
                }

                _textLoading.text = loadingText;
            }
        }
    }
}
