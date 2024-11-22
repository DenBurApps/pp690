using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class InfoScreen : MonoBehaviour
{
    [SerializeField] private PhotosController _photosController;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _gender;
    [SerializeField] private TMP_Text _date;
    [SerializeField] private TMP_Text _type;
    [SerializeField] private TMP_Text _breed;
    [SerializeField] private TMP_Text _coloring;
    [SerializeField] private TMP_Text _outside;
    [SerializeField] private TMP_Text _spayed;
    [SerializeField] private MainScreen _mainScreen;
    [SerializeField] private EditPet _edit;
    
    private ScreenVisabilityHandler _screenVisabilityHandler;

    private PetPlane _petPlane;

    public event Action<PetPlane> EditClicked;
    public event Action BackClicked; 
    
    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void OnEnable()
    {
        _mainScreen.InfoClicked += Enable;
        _edit.BackClicked += _screenVisabilityHandler.EnableScreen;
        _edit.Edited += Enable;
    }

    private void OnDisable()
    {
        _mainScreen.InfoClicked -= Enable;
        _edit.BackClicked -= _screenVisabilityHandler.EnableScreen;
        _edit.Edited -= Enable;
    }

    private void Start()
    {
        _screenVisabilityHandler.DisableScreen();
    }

    public void OnBackClicked()
    {
        BackClicked?.Invoke();
        _screenVisabilityHandler.DisableScreen();
    }

    public void OnEditClicked()
    {
        EditClicked?.Invoke(_petPlane);
        _screenVisabilityHandler.DisableScreen();
    }
    
    private void Enable(PetPlane plane)
    {
        _screenVisabilityHandler.EnableScreen();
        
        _petPlane = plane;

        _name.text = _petPlane.PetData.Name;
        _gender.text = _petPlane.PetData.IsMale ? "Male" : "Female";
        _date.text = _petPlane.PetData.BirthDate.ToString("dd MMMM yyyy");
        _type.text = _petPlane.PetData.PetType;
        _breed.text = _petPlane.PetData.Breed;
        _coloring.text = _petPlane.PetData.Coloring;
        _outside.text = _petPlane.PetData.IsIndoors ? "Indoors" : "Outside";
        _spayed.text = _petPlane.PetData.IsSpayed ? "Yes" : "No";

        if (_petPlane.PetData.ImagePath != null)
        {
            _photosController.SetPhotos(_petPlane.PetData.ImagePath);
        }
        else
        {
            _photosController.ResetPhotos();
        }
    }
}
