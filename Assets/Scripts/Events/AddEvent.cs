using System;
using Bitsplash.DatePicker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class AddEvent : MonoBehaviour
{
    [SerializeField] private TMP_InputField _eventInput;
    [SerializeField] private GameObject _dateScreen;
    [SerializeField] private DatePickerSettings _datePicker;
    [SerializeField] private TMP_Text _dateText;
    [SerializeField] private TMP_Text _datePlaceholder;
    [SerializeField] private Button _saveButton;
    [SerializeField] private PhotosController _photosController;
    [SerializeField] private EventScreen _eventScreen;
    
    private ScreenVisabilityHandler _screenVisabilityHandler;
    private string _eventKind;
    private DateTime _date;
    private byte[] _photo;
    private EventData _currentEventData;

    private Action<EventData> _onEventEdited;
    public event Action<EventData> Saved;
    public event Action BackClicked;
    
    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }
    
    private void OnEnable()
    {
        _datePicker.Content.OnSelectionChanged.AddListener(SetDate);
        _eventInput.onValueChanged.AddListener(SetKind);
        
        _saveButton.onClick.AddListener(OnSaved);

        _eventScreen.AddEvent += EnableForNewEvent;
    }

    private void OnDisable()
    {
        _datePicker.Content.OnSelectionChanged.RemoveListener(SetDate);
        _eventInput.onValueChanged.RemoveListener(SetKind);
        
        _saveButton.onClick.RemoveListener(OnSaved);
        
        _eventScreen.AddEvent -= EnableForNewEvent;
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
    
    public void EditEvent(EventData eventData, Action<EventData> onEventEdited)
    {
        _currentEventData = eventData;
        _onEventEdited = onEventEdited;

        PopulateFields(eventData);
        _screenVisabilityHandler.EnableScreen();
        ValidateInput();
    }
    
    private void PopulateFields(EventData eventData)
    {
        _eventKind = eventData.Kind;
        _eventInput.text = _eventKind;

        _date = eventData.Date;
        _dateText.text = _date.ToString("dd MMM yyyy");
        _datePlaceholder.enabled = false;

        if (eventData.ImagePath != null)
        {
            _photosController.SetPhotos(eventData.ImagePath);
        }
        else
        {
            _photosController.ResetPhotos();
        }
    }

    private void EnableForNewEvent()
    {
        Reset();
        _screenVisabilityHandler.EnableScreen();
        ValidateInput();
    }

    private void SetKind(string text)
    {
        _eventKind = text;
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
    
    private void ValidateInput()
    {
        _saveButton.interactable = !string.IsNullOrEmpty(_eventKind) && _date != default;
    }

    private void OnSaved()
    {
        if (_currentEventData == null)
        {
            var data = new EventData(_eventKind, _date, _photosController.GetPhoto());
            Saved?.Invoke(data);
        }
        else
        {
            _currentEventData.Kind = _eventKind;
            _currentEventData.Date = _date;
            _currentEventData.ImagePath = _photosController.GetPhoto();

            _onEventEdited?.Invoke(_currentEventData);
        }

        _screenVisabilityHandler.DisableScreen();
    }
    
    private void Reset()
    {
        _currentEventData = null;
        _onEventEdited = null;
        
        _eventKind = string.Empty;
        _eventInput.text = _eventKind;
        _date = default;
        _dateText.text = string.Empty;
        _datePlaceholder.enabled = true;
        _photosController.ResetPhotos();
        
        _dateScreen.SetActive(false);
    }
}
