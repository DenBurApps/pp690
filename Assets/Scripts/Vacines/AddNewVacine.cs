using System;
using System.Collections;
using System.Collections.Generic;
using Bitsplash.DatePicker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class AddNewVacine : MonoBehaviour
{
    [SerializeField] private TMP_InputField _kindInput;
    [SerializeField] private TMP_Text _dateText;
    [SerializeField] private TMP_Text _datePlaceholder;
    [SerializeField] private GameObject _dateScreen;
    [SerializeField] private DatePickerSettings _datePicker;
    [SerializeField] private Button _saveButton;
    [SerializeField] private VacinesScreen _vacinesScreen;

    private string _kind;
    private DateTime _date;
    private ScreenVisabilityHandler _screenVisabilityHandler;

    public event Action<VaccineData> Saved;
    public event Action BackClicked;

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void OnEnable()
    {
        _datePicker.Content.OnSelectionChanged.AddListener(SetDate);
        _kindInput.onValueChanged.AddListener(SetKind);
        
        _saveButton.onClick.AddListener(OnSaved);

        _vacinesScreen.AddVacine += Enable;
    }

    private void OnDisable()
    {
        _datePicker.Content.OnSelectionChanged.RemoveListener(SetDate);
        _kindInput.onValueChanged.RemoveListener(SetKind);
        
        _saveButton.onClick.RemoveListener(OnSaved);
        
        _vacinesScreen.AddVacine -= Enable;
    }

    private void Start()
    {
        _screenVisabilityHandler.DisableScreen();
        Reset();
    }

    public void OnBackClicked()
    {
        BackClicked?.Invoke();
        _screenVisabilityHandler.DisableScreen();
    }

    private void Enable()
    {
        Reset();
        _screenVisabilityHandler.EnableScreen();
        ValidateInput();
    }

    private void SetDate()
    {
        var selection = _datePicker.Content.Selection;

        _date = selection.GetItem(0);
        _dateText.text = _date.ToString("dd MMM yyyy");
        _datePlaceholder.enabled = false;

        _dateScreen.gameObject.SetActive(false);
        ValidateInput();
    }

    private void SetKind(string text)
    {
        _kind = text;
        ValidateInput();
    }

    private void ValidateInput()
    {
        _saveButton.interactable = !string.IsNullOrEmpty(_kind) && _date != default;
    }

    private void OnSaved()
    {
        var data = new VaccineData(_kind, _date);
        
        Saved?.Invoke(data);
        _screenVisabilityHandler.DisableScreen();
    }

    private void Reset()
    {
        _kind = string.Empty;
        _kindInput.text = _kind;
        _date = default;
        _dateText.text = string.Empty;
        _datePlaceholder.enabled = true;

        
        _dateScreen.SetActive(false);
    }
}