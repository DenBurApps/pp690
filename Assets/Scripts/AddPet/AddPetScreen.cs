using System;
using Bitsplash.DatePicker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class AddPetScreen : MonoBehaviour
{
    [SerializeField] private Sprite _selectedSprite;
    [SerializeField] private Sprite _defaultSprite;

    [SerializeField] private Button _backButton;
    [SerializeField] private TMP_InputField _nameInput;
    [SerializeField] private TMP_Text _dateText;
    [SerializeField] private TMP_Text _datePlaceholder;
    [SerializeField] private Button _dateButton;
    [SerializeField] private GameObject _dateScreen;
    [SerializeField] private DatePickerSettings _datePicker;
    [SerializeField] private TMP_InputField _petTypeInput;
    [SerializeField] private TMP_InputField _breedInput;
    [SerializeField] private TMP_InputField _coloringInput;
    [SerializeField] private PhotosController _photosController;
    [SerializeField] private Button _saveButton;
    [SerializeField] private MainScreen _mainScreen;

    [SerializeField] private Button _male;
    [SerializeField] private Button _female;
    [SerializeField] private Button _indoors;
    [SerializeField] private Button _outside;
    [SerializeField] private Button _yes;
    [SerializeField] private Button _no;

    private string _name;
    private DateTime _birthDate;
    private string _petType;
    private string _breed;
    private bool _isMale;
    private string _coloring;
    private bool _isSpayed;
    private bool _isIndoors;
    private byte[] _photo;

    private ScreenVisabilityHandler _screenVisabilityHandler;
    private Button _currentGender;
    private Button _currentIndors;
    private Button _yesNoButon;

    public event Action<PetData> Saved;
    public event Action BackClicked;

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void Start()
    {
        ResetData();
        ValidateInput();
        _screenVisabilityHandler.DisableScreen();
    }

    private void OnEnable()
    {
        _nameInput.onValueChanged.AddListener(SetName);
        _petTypeInput.onValueChanged.AddListener(SetPetType);
        _breedInput.onValueChanged.AddListener(SetBreed);
        _coloringInput.onValueChanged.AddListener(SetColoring);

        _datePicker.Content.OnSelectionChanged.AddListener(SetDate);

        _saveButton.onClick.AddListener(OnSaveClicked);
        _backButton.onClick.AddListener(OnBackClicked);
        _mainScreen.AddPetClicked += EnableScreen;

        _male.onClick.AddListener((() => SetGender(_male)));
        _female.onClick.AddListener((() => SetGender(_female)));

        _indoors.onClick.AddListener((() => SetIndoors(_indoors)));
        _outside.onClick.AddListener((() => SetIndoors(_outside)));

        _yes.onClick.AddListener((() => SetYesNo(_yes)));
        _no.onClick.AddListener((() => SetYesNo(_no)));
    }

    private void OnDisable()
    {
        _nameInput.onValueChanged.RemoveListener(SetName);
        _petTypeInput.onValueChanged.RemoveListener(SetPetType);
        _breedInput.onValueChanged.RemoveListener(SetBreed);
        _coloringInput.onValueChanged.RemoveListener(SetColoring);

        _datePicker.Content.OnSelectionChanged.RemoveListener(SetDate);

        _saveButton.onClick.RemoveListener(OnSaveClicked);
        _backButton.onClick.RemoveListener(OnBackClicked);
        _mainScreen.AddPetClicked -= EnableScreen;

        _male.onClick.RemoveListener((() => SetGender(_male)));
        _female.onClick.RemoveListener((() => SetGender(_female)));

        _indoors.onClick.RemoveListener((() => SetIndoors(_indoors)));
        _indoors.onClick.RemoveListener((() => SetIndoors(_outside)));

        _yes.onClick.RemoveListener((() => SetYesNo(_yes)));
        _no.onClick.RemoveListener((() => SetYesNo(_no)));
    }

    private void EnableScreen()
    {
        ResetData();
        _screenVisabilityHandler.EnableScreen();
    }

    private void SetName(string text)
    {
        _name = text;
        ValidateInput();
    }

    private void SetPetType(string text)
    {
        _petType = text;
        ValidateInput();
    }

    private void SetBreed(string text)
    {
        _breed = text;
        ValidateInput();
    }

    private void SetColoring(string text)
    {
        _coloring = text;
        ValidateInput();
    }

    private void SetGender(Button button)
    {
        if (_currentGender != null)
        {
            _currentGender.image.sprite = _defaultSprite;
        }

        _currentGender = button;
        _currentGender.image.sprite = _selectedSprite;

        _isMale = button == _male;
    }

    private void SetIndoors(Button button)
    {
        if (_currentIndors != null)
        {
            _currentIndors.image.sprite = _defaultSprite;
        }

        _currentIndors = button;
        _currentIndors.image.sprite = _selectedSprite;

        _isIndoors = button == _indoors;
    }

    private void SetYesNo(Button button)
    {
        if (_yesNoButon != null)
        {
            _yesNoButon.image.sprite = _defaultSprite;
        }

        _yesNoButon = button;
        _yesNoButon.image.sprite = _selectedSprite;

        _isSpayed = button == _yes;
    }

    private void SetDate()
    {
        var selection = _datePicker.Content.Selection;

        _birthDate = selection.GetItem(0);
        _dateText.text = _birthDate.ToString("dd MMM yyyy");
        _datePlaceholder.enabled = false;

        _dateScreen.gameObject.SetActive(false);
        ValidateInput();
    }

    private void ValidateInput()
    {
        _saveButton.interactable = !string.IsNullOrEmpty(_name) && !string.IsNullOrEmpty(_petType) &&
                                   !string.IsNullOrEmpty(_breed)
                                   && !string.IsNullOrEmpty(_coloring) && _birthDate != default;
    }

    private void OnSaveClicked()
    {
        _photo = _photosController.GetPhoto();

        var saveData = new PetData(_name, _birthDate, _petType, _breed, _coloring, _isMale, _isIndoors, _isSpayed,
            _photo);

        Saved?.Invoke(saveData);
        ResetData();
        _screenVisabilityHandler.DisableScreen();
    }

    private void ResetData()
    {
        _name = string.Empty;
        _nameInput.text = _name;
        _birthDate = default;
        _datePlaceholder.enabled = true;
        _dateText.text = string.Empty;
        _petType = string.Empty;
        _petTypeInput.text = _petType;
        _breed = string.Empty;
        _breedInput.text = _breed;
        _coloring = string.Empty;
        _coloringInput.text = _coloring;
        _isMale = false;
        _isSpayed = false;
        _photo = null;
        _photosController.ResetPhotos();
        _dateScreen.gameObject.SetActive(false);
    }

    private void OnBackClicked()
    {
        ResetData();
        BackClicked?.Invoke();
        _screenVisabilityHandler.DisableScreen();
    }
}