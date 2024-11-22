using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class VacinesScreen : MonoBehaviour
{
    [SerializeField] private List<VacinePlane> _vacinePlanes;
    [SerializeField] private MainScreen _mainScreen;
    [SerializeField] private GameObject _emptyPlane;
    [SerializeField] private AddNewVacine _addNewVacine;
    
    private ScreenVisabilityHandler _screenVisabilityHandler;
    private PetPlane _petPlane;

    public event Action AddVacine;
    public event Action BackClicked;

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void Start()
    {
        _screenVisabilityHandler.DisableScreen();
    }

    private void OnEnable()
    {
        foreach (var plane in _vacinePlanes)
        {
            plane.Deleted += DeleteVacine;
        }

        _mainScreen.OpenVacines += Enable;
        _addNewVacine.Saved += AddNewVacine;
        _addNewVacine.BackClicked += _screenVisabilityHandler.EnableScreen;
    }

    private void OnDisable()
    {
        foreach (var plane in _vacinePlanes)
        {
            plane.Deleted -= DeleteVacine;
        }

        _mainScreen.OpenVacines -= Enable;
        _addNewVacine.Saved -= AddNewVacine;
        _addNewVacine.BackClicked -= _screenVisabilityHandler.EnableScreen;
    }

    public void OnAddVacineClicked()
    {
        AddVacine?.Invoke();
        _screenVisabilityHandler.DisableScreen();
    }

    public void OnBackClicked()
    {
        BackClicked?.Invoke();
        _screenVisabilityHandler.DisableScreen();
    }

    private void Enable(PetPlane plane)
    {
        DisableAllPlanes();
        _screenVisabilityHandler.EnableScreen();

        _petPlane = plane;

        if (_petPlane.PetData.VaccineDatas.Count <= 0)
        {
            ToggleEmptyPlane();
            return;
        }

        foreach (var data in _petPlane.PetData.VaccineDatas)
        {
            var availablePlane = _vacinePlanes.FirstOrDefault(plane => !plane.IsActive);

            if (availablePlane != null)
                availablePlane.Enable(data);
        }
        
        ToggleEmptyPlane();
    }

    private void AddNewVacine(VaccineData data)
    {
        _screenVisabilityHandler.EnableScreen();

        var availablePlane = _vacinePlanes.FirstOrDefault(plane => !plane.IsActive);

        if (availablePlane != null)
            availablePlane.Enable(data);

        if (!_petPlane.PetData.VaccineDatas.Contains(data))
        {
            _petPlane.PetData.VaccineDatas.Add(data);
            _petPlane.UpdateUI();
        }
        
        ToggleEmptyPlane();
    }

    private void DeleteVacine(VacinePlane plane)
    {
        if (_petPlane.PetData.VaccineDatas.Contains(plane.Data))
        {
            _petPlane.PetData.VaccineDatas.Remove(plane.Data);
            _petPlane.UpdateUI();
        }
        
        plane.Reset();
        plane.Disable();
        ToggleEmptyPlane();
    }

    private void ToggleEmptyPlane()
    {
        _emptyPlane.SetActive(_vacinePlanes.All(plane => !plane.IsActive));
    }

    private void DisableAllPlanes()
    {
        foreach (var plane in _vacinePlanes)
        {
            plane.Reset();
            plane.Disable();
        }
    }
}