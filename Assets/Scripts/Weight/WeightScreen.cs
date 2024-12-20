using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class WeightScreen : MonoBehaviour
{
    [SerializeField] private List<WeightPlane> _weightPlanes;
    [SerializeField] private MainScreen _mainScreen;
    [SerializeField] private GameObject _emptyPlane;
    [SerializeField] private AddWeightScreen _addWeightScreen;

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
            plane.Deleted += DeleteWeight;
        }

        _mainScreen.OpenWeight += Enable;
        _addWeightScreen.Saved += AddNewWeight;
        _addWeightScreen.BackClicked += _screenVisabilityHandler.EnableScreen;
    }

    private void OnDisable()
    {
        foreach (var plane in _weightPlanes)
        {
            plane.Deleted -= DeleteWeight;
        }

        _mainScreen.OpenWeight -= Enable;
        _addWeightScreen.Saved -= AddNewWeight;
        _addWeightScreen.BackClicked -= _screenVisabilityHandler.EnableScreen;
    }

    public void OnAddWeightClicked()
    {
        AddWeight?.Invoke();
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

        if (_petPlane.PetData.WeightDatas.Count <= 0)
        {
            ToggleEmptyPlane();
            return;
        }

        foreach (var data in _petPlane.PetData.WeightDatas)
        {
            var availablePlane = _weightPlanes.FirstOrDefault(plane => !plane.IsActive);

            if (availablePlane != null)
                availablePlane.Enable(data);
        }

        ToggleEmptyPlane();
    }

    private void AddNewWeight(WeightData data)
    {
        _screenVisabilityHandler.EnableScreen();

        var availablePlane = _weightPlanes.FirstOrDefault(plane => !plane.IsActive);

        if (availablePlane != null)
            availablePlane.Enable(data);

        if (!_petPlane.PetData.WeightDatas.Contains(data))
        {
            _petPlane.PetData.WeightDatas.Add(data);
            _petPlane.UpdateUI();
        }

        ToggleEmptyPlane();
    }

    private void DeleteWeight(WeightPlane plane)
    {
        if (_petPlane.PetData.WeightDatas.Contains(plane.Data))
        {
            _petPlane.PetData.WeightDatas.Remove(plane.Data);
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