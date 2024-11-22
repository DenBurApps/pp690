using System;
using Bitsplash.DatePicker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class AddWeightScreen : MonoBehaviour
{
    [SerializeField] private Sprite _defaultManAndPetSprite;
    [SerializeField] private Sprite _selectedManAndPetSprite;
    [SerializeField] private Sprite _defaultOnlyPetSprite;
    [SerializeField] private Sprite _selectedOnlyPetSprite;

    [SerializeField] private TMP_InputField _totalWeightOwnerPetInput;
    [SerializeField] private TMP_InputField _ownerWeightInput;
    [SerializeField] private TMP_InputField _petWeightInput;
    [SerializeField] private TMP_Text _ownerPetDateText;
    [SerializeField] private TMP_Text _ownerPetDatePlaceholder;
    [SerializeField] private TMP_Text _petWeightDateText;
    [SerializeField] private TMP_Text _petWeightDatePlaceholder;

    [SerializeField] private Button _manAndPetButton;
    [SerializeField] private Button _onlyPetButton;
    [SerializeField] private GameObject _manAndPetPlane;
    [SerializeField] private GameObject _onlyPetPlane;
    [SerializeField] private GameObject _dateScreen;
    [SerializeField] private DatePickerSettings _datePicker;
    [SerializeField] private WeightScreen _weightScreen;

    [SerializeField] private Button _saveButton;

    private Button _currentButton;
    private bool _isPetOnly;
    private string _totalWeightOwnerPet;
    private string _totalWeightPetOnly;
    private string _ownerWeight;
    private DateTime _date;

    private ScreenVisabilityHandler _screenVisabilityHandler;

    public event Action BackClicked;
    public event Action<WeightData> Saved; 
    
    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void OnEnable()
    {
        _manAndPetButton.onClick.AddListener(OnManAndPetButtonClicked);
        _onlyPetButton.onClick.AddListener(OnOnlyPetButtonClicked);

        _datePicker.Content.OnSelectionChanged.AddListener(SetDate);
        _saveButton.onClick.AddListener(Save);
        
        _totalWeightOwnerPetInput.onValueChanged.AddListener(SetTotalWeightOwnerPet);
        _ownerWeightInput.onValueChanged.AddListener(SetOwnerWeight);
        
        _petWeightInput.onValueChanged.AddListener(SetTotalWeightPetOnly);
        _weightScreen.AddWeight += Enable;
    }

    private void OnDisable()
    {
        _manAndPetButton.onClick.RemoveListener(OnManAndPetButtonClicked);
        _onlyPetButton.onClick.RemoveListener(OnOnlyPetButtonClicked);

        _datePicker.Content.OnSelectionChanged.RemoveListener(SetDate);
        _saveButton.onClick.RemoveListener(Save);
        
        _totalWeightOwnerPetInput.onValueChanged.RemoveListener(SetTotalWeightOwnerPet);
        _ownerWeightInput.onValueChanged.RemoveListener(SetOwnerWeight);
        
        _petWeightInput.onValueChanged.RemoveListener(SetTotalWeightPetOnly);
        _weightScreen.AddWeight -= Enable;
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
        SelectButton(_manAndPetButton);
        ValidateInput();
    }

    private void SetTotalWeightPetOnly(string text)
    {
        _totalWeightPetOnly = text;
        ValidateInput();
    }

    private void SetTotalWeightOwnerPet(string text)
    {
        _totalWeightOwnerPet = text;
        ValidateInput();
    }

    private void SetOwnerWeight(string text)
    {
        _ownerWeight = text;
        ValidateInput();
    }
    
    private void SelectButton(Button button)
    {
        if (_currentButton != null)
        {
            ResetButton(_currentButton);
        }

        _currentButton = button;

        if (_currentButton == _manAndPetButton)
        {
            _manAndPetButton.image.sprite = _selectedManAndPetSprite;
            _manAndPetPlane.gameObject.SetActive(true);
            _onlyPetPlane.gameObject.SetActive(false);
            _isPetOnly = false;
        }
        else
        {
            _onlyPetButton.image.sprite = _selectedOnlyPetSprite;
            _manAndPetPlane.gameObject.SetActive(false);
            _onlyPetPlane.gameObject.SetActive(true);
            _isPetOnly = true;
        }
        
        ValidateInput();
    }

    private void ResetButton(Button button)
    {
        if (button == _manAndPetButton)
        {
            _manAndPetButton.image.sprite = _defaultManAndPetSprite;
            _totalWeightOwnerPet = string.Empty;
            _totalWeightOwnerPetInput.text = _totalWeightOwnerPet;
            _ownerWeight = string.Empty;
            _ownerWeightInput.text = _ownerWeight;
            _ownerPetDateText.text = string.Empty;
            _ownerPetDatePlaceholder.enabled = true;
        }
        else if (button == _onlyPetButton)
        {
            _onlyPetButton.image.sprite = _defaultOnlyPetSprite;
            _totalWeightPetOnly = string.Empty;
            _petWeightInput.text = string.Empty;
            _petWeightDateText.text = string.Empty;
            _petWeightDatePlaceholder.enabled = true;
        }
        
        ValidateInput();
    }

    private void SetDate()
    {
        var selection = _datePicker.Content.Selection;

        _date = selection.GetItem(0);
        string formattedDate = _date.ToString("dd MMM yyyy");

        if (_isPetOnly)
        {
            _petWeightDateText.text = formattedDate;
            _petWeightDatePlaceholder.enabled = false;
        }
        else
        {
            _ownerPetDateText.text = formattedDate;
            _ownerPetDatePlaceholder.enabled = false;
        }

        ValidateInput();
        _dateScreen.SetActive(false);
    }

    private void ValidateInput()
    {
        bool isValid;

        if (_isPetOnly)
        {
            isValid = !string.IsNullOrEmpty(_totalWeightPetOnly) && _date != default;
        }
        else
        {
            isValid = !string.IsNullOrEmpty(_totalWeightOwnerPet) && _date != default && !string.IsNullOrEmpty(_ownerWeight);
        }

        _saveButton.interactable = isValid;
    }

    private void Save()
    {
        WeightData dataToSave;
        
        if (_isPetOnly)
        {
            dataToSave = new WeightData(_isPetOnly, _totalWeightPetOnly, string.Empty, _date);
        }
        else
        {
            dataToSave = new WeightData(_isPetOnly, _totalWeightOwnerPet, _ownerWeight, _date);
        }
        
        Saved?.Invoke(dataToSave);
        _screenVisabilityHandler.DisableScreen();
    }

    private void Reset()
    {
        _totalWeightOwnerPet = string.Empty;
        _totalWeightPetOnly = string.Empty;
        _ownerWeightInput.text = _totalWeightOwnerPet;
        _petWeightInput.text = _totalWeightOwnerPet;
        _totalWeightOwnerPetInput.text = _totalWeightOwnerPet;
        _ownerWeight = string.Empty;
        
        _petWeightDateText.text = string.Empty;
        _petWeightDatePlaceholder.enabled = true;

        _ownerPetDateText.text = string.Empty;
        _ownerPetDatePlaceholder.enabled = true;
        
        ResetButton(_manAndPetButton);
        ResetButton(_onlyPetButton);

        _date = default;
        _dateScreen.SetActive(false);
        ValidateInput();
    }
    
    private void OnManAndPetButtonClicked() => SelectButton(_manAndPetButton);
    private void OnOnlyPetButtonClicked() => SelectButton(_onlyPetButton);
}