using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class Settings : MonoBehaviour
{
    [SerializeField] private GameObject _settingsCanvas;
    [SerializeField] private GameObject _privacyCanvas;
    [SerializeField] private GameObject _termsCanvas;
    [SerializeField] private GameObject _contactCanvas;
    [SerializeField] private GameObject _versionCanvas;
    [SerializeField] private TMP_Text _versionText;
    [SerializeField] private MainScreen _mainScreen;
    [SerializeField] private EventsHomeScreen _eventsHomeScreen;
    private string _version = "Application version:\n";

    public event Action HomeScreenClicked;
    public event Action EventsScreenClicked;
    
    private void Awake()
    {
        _settingsCanvas.SetActive(false);
        _privacyCanvas.SetActive(false);
        _termsCanvas.SetActive(false);
        _contactCanvas.SetActive(false);
        _versionCanvas.SetActive(false);
        SetVersion();
    }

    private void OnEnable()
    {
        _mainScreen.SettingsClicked += ShowSettings;
        _eventsHomeScreen.SettingsClicked += ShowSettings;
    }

    private void OnDisable()
    {
        _mainScreen.SettingsClicked -= ShowSettings;
        _eventsHomeScreen.SettingsClicked -= ShowSettings;
    }

    public void OpenHomeScreen()
    {
        HomeScreenClicked?.Invoke();
        _settingsCanvas.SetActive(false);
    }

    public void OnEventsClicked()
    {
        EventsScreenClicked?.Invoke();
        _settingsCanvas.SetActive(false);
    }

    private void SetVersion()
    {
        _versionText.text = _version + Application.version;
    }

    public void ShowSettings()
    {
        _settingsCanvas.SetActive(true);
    }

    public void RateUs()
    {
#if UNITY_IOS
        Device.RequestStoreReview();
#endif
    }
}
