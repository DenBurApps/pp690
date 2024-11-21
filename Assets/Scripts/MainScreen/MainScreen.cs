using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class MainScreen : MonoBehaviour
{
    [SerializeField] private List<PetPlane> _petPlanes;
    [SerializeField] private AddPetScreen _addPetScreen;
    [SerializeField] private GameObject _emptyPlane;
    [SerializeField] private AddImageScreen _addImageScreen;
    [SerializeField] private VacinesScreen _vacinesScreen;

    private ScreenVisabilityHandler _screenVisabilityHandler;

    public event Action AddPetClicked;
    public event Action<PetPlane> OpenVacines;
    public event Action<PetPlane> OpenWeight; 

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void OnEnable()
    {
        _addPetScreen.BackClicked += EnableScreen;
        _addPetScreen.Saved += EnablePlane;
        _vacinesScreen.BackClicked += EnableScreen;

        foreach (var plane in _petPlanes)
        {
            plane.PhotoButtonClicked += EnableImageScreen;
            plane.VacineButtonClicked += OnOpenVacines;
        }
    }

    private void OnDisable()
    {
        _addPetScreen.BackClicked -= EnableScreen;
        _addPetScreen.Saved -= EnablePlane;
        _vacinesScreen.BackClicked -= EnableScreen;

        foreach (var plane in _petPlanes)
        {
            plane.PhotoButtonClicked -= EnableImageScreen;
            plane.VacineButtonClicked -= OnOpenVacines;
        }
    }

    private void Start()
    {
        _screenVisabilityHandler.EnableScreen();
        DisableAllPlanes();
        ToggleEmptyPlane();
        _addImageScreen.Disable();
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
}