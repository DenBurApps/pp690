using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class EventScreen : MonoBehaviour
{
    [SerializeField] private List<EventPlane> _eventPlanes;
    [SerializeField] private MainScreen _mainScreen;
    [SerializeField] private GameObject _emptyPlane;
    [SerializeField] private AddEvent _addEvent;
    
    private ScreenVisabilityHandler _screenVisabilityHandler;
    private PetPlane _petPlane;

    public event Action AddEvent;
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
        foreach (var plane in _eventPlanes)
        {
            plane.Deleted += DeleteVacine;
        }

        _mainScreen.OpenEvents += Enable;
        _addEvent.Saved += OnAddEvent;
        _addEvent.BackClicked += _screenVisabilityHandler.EnableScreen;
    }

    private void OnDisable()
    {
        foreach (var plane in _eventPlanes)
        {
            plane.Deleted -= DeleteVacine;
        }

        _mainScreen.OpenEvents -= Enable;
        _addEvent.Saved -= OnAddEvent;
        _addEvent.BackClicked -= _screenVisabilityHandler.EnableScreen;
    }

    public void OnAddEventClicked()
    {
        AddEvent?.Invoke();
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

        if (_petPlane.PetData.EventDatas.Count <= 0)
        {
            ToggleEmptyPlane();
            return;
        }

        foreach (var data in _petPlane.PetData.EventDatas)
        {
            var availablePlane = _eventPlanes.FirstOrDefault(plane => !plane.IsActive);

            if (availablePlane != null)
                availablePlane.Enable(data);
        }
        
        ToggleEmptyPlane();
    }

    private void OnAddEvent(EventData data)
    {
        _screenVisabilityHandler.EnableScreen();

        var availablePlane = _eventPlanes.FirstOrDefault(plane => !plane.IsActive);

        if (availablePlane != null)
            availablePlane.Enable(data);

        if (!_petPlane.PetData.EventDatas.Contains(data))
        {
            _petPlane.PetData.EventDatas.Add(data);
            _petPlane.UpdateUI();
        }
        
        ToggleEmptyPlane();
    }

    private void DeleteVacine(EventPlane plane)
    {
        if (_petPlane.PetData.EventDatas.Contains(plane.Data))
        {
            _petPlane.PetData.EventDatas.Remove(plane.Data);
            _petPlane.UpdateUI();
        }
        
        plane.Reset();
        plane.Disable();
        ToggleEmptyPlane();
    }

    private void ToggleEmptyPlane()
    {
        _emptyPlane.SetActive(_eventPlanes.All(plane => !plane.IsActive));
    }

    private void DisableAllPlanes()
    {
        foreach (var plane in _eventPlanes)
        {
            plane.Reset();
            plane.Disable();
        }
    }
}
