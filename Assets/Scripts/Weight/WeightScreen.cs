using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class WeightScreen : MonoBehaviour
{
    [SerializeField] private List<WeightPlane> _weightPlanes;
    [SerializeField] private MainScreen _mainScreen;
    [SerializeField] private GameObject _emptyPlane;
    //[SerializeField] private AddNewVacine _addNewVacine;
    
    private ScreenVisabilityHandler _screenVisabilityHandler;
    private PetPlane _petPlane;

    public event Action AddWeight;
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
        foreach (var plane in _weightPlanes)
        {
            plane.Deleted += DeleteVacine;
        }

        _mainScreen.OpenVacines += Enable;
        _addNewVacine.Saved += AddNewVacine;
    }

    private void OnDisable()
    {
        foreach (var plane in _weightPlanes)
        {
            plane.Deleted -= DeleteVacine;
        }

        _mainScreen.OpenVacines -= Enable;
        _addNewVacine.Saved -= AddNewVacine;
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
            var availablePlane = _weightPlanes.FirstOrDefault(plane => !plane.IsActive);

            if (availablePlane != null)
                availablePlane.Enable(data);
        }
        
        ToggleEmptyPlane();
    }

    private void AddNewVacine(VaccineData data)
    {
        _screenVisabilityHandler.EnableScreen();

        var availablePlane = _weightPlanes.FirstOrDefault(plane => !plane.IsActive);

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
        _emptyPlane.SetActive(_weightPlanes.All(plane => !plane.IsActive));
    }

    private void DisableAllPlanes()
    {
        foreach (var plane in _weightPlanes)
        {
            plane.Reset();
            plane.Disable();
        }
    }
}
