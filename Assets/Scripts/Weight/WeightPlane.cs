using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeightPlane : MonoBehaviour
{
    [SerializeField] private TMP_Text _weight;
    [SerializeField] private TMP_Text _date;

    public event Action<WeightPlane> Deleted;

    public bool IsActive { get; private set; }

    public WeightData Data { get; private set; }

    public void Enable(WeightData data)
    {
        gameObject.SetActive(true);
        IsActive = true;

        Data = data;

        if (Data.IsPetOnly)
        {
            _weight.text = Data.TotalWeight.ToString() + " KG";
        }
        else
        {
            _weight.text = Data.OwnerWeight + "KG/" + Data.TotalWeight + " KG";
        }

        _date.text = Data.Date.ToString("dd MMM yyyy");
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

        _weight.text = string.Empty;
        _date.text = string.Empty;
    }

    public void OnDeleted()
    {
        Deleted?.Invoke(this);
    }
}