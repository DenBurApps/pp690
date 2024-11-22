using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class EventsHomeScreen : MonoBehaviour
{
    [SerializeField] private List<EventPlane> _eventPlanes;
    [SerializeField] private MainScreen _mainScreen;
    [SerializeField] private GameObject _emptyPlane;
    [SerializeField] private AddEvent _addEvent;
    [SerializeField] private Settings _settings;

    private ScreenVisabilityHandler _screenVisabilityHandler;

    public event Action HomeScreenClicked;
    public event Action SettingsClicked;
    
    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void OnEnable()
    {
        _mainScreen.EventsClicked += Enable;
        _settings.EventsScreenClicked += Enable;
    }

    private void OnDisable()
    {
        _mainScreen.EventsClicked -= Enable;
        _settings.EventsScreenClicked -= Enable;
    }

    private void Start()
    {
        DisableAllPlanes();
        SubscribeToEventPlaneClicks();
        _screenVisabilityHandler.DisableScreen();
    }
    
    private void Enable()
    {
        DisableAllPlanes();
        _screenVisabilityHandler.EnableScreen();

        foreach (var plane in _mainScreen.PetPlanes)
        {
            if (plane.PetData != null && plane.PetData.EventDatas.Count > 0 && plane.IsActive)
            {
                foreach (var eventData in plane.PetData.EventDatas)
                {
                    EnablePlane(eventData);
                }
            }
        }

        ToggleEmptyPlane();
    }

    public void OnHomeScreenClicked()
    {
        HomeScreenClicked?.Invoke();
        _screenVisabilityHandler.DisableScreen();
    }

    public void OnSettingsClicked()
    {
        SettingsClicked?.Invoke();
        _screenVisabilityHandler.DisableScreen();
    }
    
    private void EnablePlane(EventData eventData)
    {
        var availablePlane = _eventPlanes.FirstOrDefault(plane => !plane.IsActive);

        if (availablePlane != null)
        {
            availablePlane.Enable(eventData);
        }
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
    
    private void SubscribeToEventPlaneClicks()
    {
        foreach (var plane in _eventPlanes)
        {
            plane.EventPlaneClicked += OnEventPlaneClicked;
        }
    }
    
    private void OnEventPlaneClicked(EventData eventData)
    {
        _addEvent.EditEvent(eventData, OnEventEdited);
    }


    private void OnEventEdited(EventData updatedEventData)
    {

        var eventPlane = _eventPlanes.FirstOrDefault(plane => plane.IsActive && plane.ContainsEventData(updatedEventData));
        if (eventPlane != null)
        {
            eventPlane.Enable(updatedEventData);
        }
        
        var petPlane = _mainScreen.PetPlanes.FirstOrDefault(plane => plane.PetData.EventDatas.Contains(updatedEventData));
        if (petPlane != null)
        {
            var eventIndex = petPlane.PetData.EventDatas.IndexOf(updatedEventData);
            petPlane.PetData.EventDatas[eventIndex] = updatedEventData;
            petPlane.UpdateUI();
        }

        ToggleEmptyPlane(); 
    }
}
