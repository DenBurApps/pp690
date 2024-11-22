using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventPlane : MonoBehaviour
{
    [SerializeField] private TMP_Text _kind;
    [SerializeField] private TMP_Text _date;
    [SerializeField] private PhotosController _photosController;

    public event Action<EventPlane> Deleted;
    public event Action<EventData> EventPlaneClicked;
    
    public bool IsActive { get; private set; }
   
    public EventData Data { get; private set; }
   
    public void Enable(EventData data)
    {
        gameObject.SetActive(true);
        IsActive = true;

        Data = data;

        _kind.text = Data.Kind;
        _date.text = Data.Date.ToString("dd MMM yyyy");

        if (data.ImagePath != null)
        {
            _photosController.SetPhotos(data.ImagePath);
        }
        else
        {
            _photosController.ResetPhotos();
        }
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        IsActive = false;
    }

    public void Reset()
    {
        if (Data != null)
        {
            Data = null;
        }
        _kind.text = string.Empty;
        _date.text = string.Empty;
    }
    
    public bool ContainsEventData(EventData eventData)
    {
        return Data == eventData;
    }

    public void OnDeleted()
    {
        Deleted?.Invoke(this);
    }
    
    public void OnClick()
    {
        EventPlaneClicked?.Invoke(Data);
    }
}
