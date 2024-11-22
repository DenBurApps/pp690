using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class MainScreen : MonoBehaviour
{
    [SerializeField] private List<PetPlane> _petPlanes;
    [SerializeField] private AddPetScreen _addPetScreen;
    [SerializeField] private GameObject _emptyPlane;
    [SerializeField] private AddImageScreen _addImageScreen;
    [SerializeField] private VacinesScreen _vacinesScreen;
    [SerializeField] private WeightScreen _weightScreen;
    [SerializeField] private EventScreen _eventScreen;
    [SerializeField] private InfoScreen _infoScreen;
    [SerializeField] private EventsHomeScreen _eventsHomeScreen;
    [SerializeField] private Settings _settings;

    private ScreenVisabilityHandler _screenVisabilityHandler;

    public event Action AddPetClicked;
    public event Action<PetPlane> OpenVacines;
    public event Action<PetPlane> OpenWeight;
    public event Action<PetPlane> OpenEvents;
    public event Action<PetPlane> InfoClicked;
    public event Action EventsClicked;
    public event Action SettingsClicked;

    public IReadOnlyCollection<PetPlane> PetPlanes => _petPlanes;
    
    private void Awake()
    {
        Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void OnEnable()
    {
        _addPetScreen.BackClicked += EnableScreen;
        _addPetScreen.Saved += EnablePlane;
        _vacinesScreen.BackClicked += EnableScreen;
        _weightScreen.BackClicked += EnableScreen;
        _eventScreen.BackClicked += EnableScreen;
        _infoScreen.BackClicked += EnableScreen;
        _eventsHomeScreen.HomeScreenClicked += EnableScreen;
        _settings.HomeScreenClicked += EnableScreen;
        
        foreach (var plane in _petPlanes)
        {
            plane.PhotoButtonClicked += EnableImageScreen;
            plane.VacineButtonClicked += OnOpenVacines;
            plane.WeightButtonClicked += OnOpenWeight;
            plane.EventButtonClicked += OnOpenEvents;
            plane.InfoButtonClicked += OnInfoClicked;
            plane.Edited += SaveAllPetData;
        }
    }

    private void OnDisable()
    {
        _addPetScreen.BackClicked -= EnableScreen;
        _addPetScreen.Saved -= EnablePlane;
        _vacinesScreen.BackClicked -= EnableScreen;
        _weightScreen.BackClicked -= EnableScreen;
        _eventScreen.BackClicked -= EnableScreen;
        _infoScreen.BackClicked -= EnableScreen;
        _eventsHomeScreen.HomeScreenClicked -= EnableScreen;
        _settings.HomeScreenClicked -= EnableScreen;

        foreach (var plane in _petPlanes)
        {
            plane.PhotoButtonClicked -= EnableImageScreen;
            plane.VacineButtonClicked -= OnOpenVacines;
            plane.WeightButtonClicked -= OnOpenWeight;
            plane.EventButtonClicked -= OnOpenEvents;
            plane.InfoButtonClicked -= OnInfoClicked;
            plane.Edited -= SaveAllPetData;
        }
    }

    private void Start()
    {
        _screenVisabilityHandler.EnableScreen();
        DisableAllPlanes();
        _addImageScreen.Disable();
        
        var savedPetData = PetDataSaveSystem.Load();
        foreach (var data in savedPetData)
        {
            EnablePlane(data);
        }
        
        ToggleEmptyPlane();
    }

    public void OnEventScreenClicked()
    {
        EventsClicked?.Invoke();
        _screenVisabilityHandler.DisableScreen();
    }

    public void OnSettingsClicked()
    {
        SettingsClicked?.Invoke();
        _screenVisabilityHandler.DisableScreen();
    }

    public void AddNewPet()
    {
        AddPetClicked?.Invoke();
        _screenVisabilityHandler.DisableScreen();
    }

    private void EnableScreen()
    {
        _screenVisabilityHandler.EnableScreen();

        foreach (var plane in _petPlanes)
        {
            if (plane.PetData != null)
                plane.UpdateUI();
        }
    }

    private void DisableAllPlanes()
    {
        foreach (var plane in _petPlanes)
        {
            plane.Reset();
            plane.Disable();
        }
    }

    private void EnablePlane(PetData data)
    {
        _screenVisabilityHandler.EnableScreen();

        var availablePlane = _petPlanes.FirstOrDefault(plane => !plane.IsActive);

        if (availablePlane == null)
            return;

        availablePlane.Enable(data);

        SaveAllPetData();
        ToggleEmptyPlane();
    }

    private void ToggleEmptyPlane()
    {
        _emptyPlane.gameObject.SetActive(_petPlanes.All(plane => !plane.IsActive));
    }

    private void EnableImageScreen(PetPlane plane)
    {
        _addImageScreen.Enable(plane);
    }

    private void OnOpenVacines(PetPlane plane)
    {
        OpenVacines?.Invoke(plane);
        _screenVisabilityHandler.DisableScreen();
    }

    private void OnOpenWeight(PetPlane plane)
    {
        OpenWeight?.Invoke(plane);
        _screenVisabilityHandler.DisableScreen();
    }
    
    private void OnOpenEvents(PetPlane plane)
    {
        OpenEvents?.Invoke(plane);
        _screenVisabilityHandler.DisableScreen();
    }

    private void OnInfoClicked(PetPlane plane)
    {
        InfoClicked?.Invoke(plane);
        _screenVisabilityHandler.DisableScreen();
    }
    
    private void SaveAllPetData()
    {
        var petDataList = new List<PetData>();

        foreach (var plane in _petPlanes)
        {
            if (plane.PetData != null)
            {
                petDataList.Add(plane.PetData);
            }
        }

        PetDataSaveSystem.Save(petDataList);
    }
}